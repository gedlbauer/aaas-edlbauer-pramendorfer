using AaaS.Common;
using AaaS.Dal.Ado.Utilities;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public abstract class AdoDetectorDao : IDetectorDao
    {
        private readonly AdoTemplate template;
        private readonly IClientDao clientDao;
        private readonly IActionDao actionDao;
        private readonly IObjectPropertyDao objectPropertyDao;

        public AdoDetectorDao(IConnectionFactory connectionFactory, IClientDao clientDao, IActionDao actionDao, IObjectPropertyDao objectPropertyDao)
        {
            template = new AdoTemplate(connectionFactory);
            this.clientDao = clientDao;
            this.actionDao = actionDao;
            this.objectPropertyDao = objectPropertyDao;
        }

        protected abstract string LastInsertedIdQuery { get; }

        public Task<bool> DeleteAsync(Detector obj)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Detector> FindAllAsync()
        {
            return template.QueryAsync(
                "SELECT * FROM Detector d " +
                "JOIN Object o ON (o.id = d.object_id) ",
                MapRowToDetector);
        }

        public async Task<Detector> FindByIdAsync(int id)
        {
            return await template.QuerySingleAsync(
                "SELECT * FROM Detector d " +
                "JOIN Object o ON (o.id = d.object_id) " +
                "WHERE o.id = @id "
                , MapRowToDetector,
                new QueryParameter("@id", id)
                );
        }

        public async Task InsertAsync(Detector detector)
        {
            const string SQL_INSERT_OBJECT = "INSERT INTO Object (type) VALUES (@type)";
            const string SQL_INSERT_DETECTOR =
                "INSERT INTO DETECTOR (object_id, client_id, telemetry_name, action_id, check_interval) " +
                "VALUES (@object_id, @client_id, @telemetry_name, @action_id, @check_interval)";
            detector.Id = Convert.ToInt32(
                    await template.ExecuteScalarAsync<object>($"{SQL_INSERT_OBJECT};{LastInsertedIdQuery}",
                    new QueryParameter("@type", detector.GetType().AssemblyQualifiedName)));
            if((await clientDao.FindByIdAsync(detector.Client.Id)) is null)
            {
                await clientDao.InsertAsync(detector.Client);
            }
            if((await actionDao.FindByIdAsync(detector.Action.Id)) is null)
            {
                await actionDao.InsertAsync(detector.Action);
            }
            await template.ExecuteScalarAsync<object>($"{SQL_INSERT_DETECTOR}; {LastInsertedIdQuery}",
                new QueryParameter("@object_id", detector.Id),
                new QueryParameter("@client_id", detector.Client.Id),
                new QueryParameter("@telemetry_name", detector.TelemetryName),
                new QueryParameter("@action_id", detector.Action.Id),
                new QueryParameter("@check_interval", detector.CheckInterval.TotalMilliseconds)
                );
            await InsertProperties(detector);
        }

        private async Task InsertProperties(Detector detector)
        {
            var staticDetectorProperties = typeof(Detector).GetProperties();
            var properties = detector.GetType().GetProperties()
                .Where(x => !staticDetectorProperties.Any(y => x.Name == y.Name && x.PropertyType == y.PropertyType));
            foreach (var property in properties)
            {
                var propName = property.Name;
                var propTypeName = property.PropertyType.AssemblyQualifiedName;
                var propValue = property.GetValue(detector);
                var objectProperty = new ObjectProperty { ObjectId = detector.Id, Name = propName, TypeName = propTypeName, Value = JsonSerializer.Serialize(propValue) };
                await objectPropertyDao.InsertAsync(objectProperty);
            }
        }

        public async Task<Detector> MapRowToDetector(IDataRecord record)
        {
            string typeName = (string)record["type"];
            var type = Type.GetType(typeName);
            var detector = (Detector)Activator.CreateInstance(type);
            detector.Id = (int)record["id"];
            detector.TelemetryName = (string)record["telemetry_name"];
            detector.CheckInterval = TimeSpan.FromMilliseconds((int)record["check_interval"]);
            detector.Client = await clientDao.FindByIdAsync((int)record["client_id"]);
            detector.Action = await actionDao.FindByIdAsync((int)record["action_id"]);
            return detector;
        }

        public Task<bool> UpdateAsync(Detector obj)
        {
            throw new NotImplementedException();
        }
    }
}
