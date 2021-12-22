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
using System.Transactions;

namespace AaaS.Dal.Ado
{
    public abstract class AdoDetectorDao<TDetector, TAction> : IDetectorDao<TDetector, TAction>
        where TDetector : Detector<TAction>
        where TAction : AaaSAction
    {
        private readonly AdoTemplate template;
        private readonly IClientDao clientDao;
        private readonly IActionDao<TAction> actionDao;
        private readonly IObjectPropertyDao objectPropertyDao;

        public AdoDetectorDao(IConnectionFactory connectionFactory, IClientDao clientDao, IActionDao<TAction> actionDao, IObjectPropertyDao objectPropertyDao)
        {
            template = new AdoTemplate(connectionFactory);
            this.clientDao = clientDao;
            this.actionDao = actionDao;
            this.objectPropertyDao = objectPropertyDao;
        }

        protected abstract string LastInsertedIdQuery { get; }

        public async Task<bool> DeleteAsync(TDetector obj)
        {
            const string SQL_DELETE_DETECTOR = "DELETE FROM Detector WHERE object_id=@object_id";
            const string SQL_DELETE_OBJECT = "DELETE FROM Object WHERE id=@id";
            int deletedRows = await template.ExecuteAsync(SQL_DELETE_DETECTOR, new QueryParameter("@object_id", obj.Id));
            if (deletedRows > 0)
            {
                await template.ExecuteAsync(SQL_DELETE_OBJECT, new QueryParameter("@id", obj.Id));
                return true;
            }
            return false;
        }

        public IAsyncEnumerable<TDetector> FindAllAsync()
        {
            return template.QueryAsync(
                "SELECT * FROM Detector d " +
                "JOIN Object o ON (o.id = d.object_id) ",
                MapRowToDetector);
        }

        public async Task<TDetector> FindByIdAsync(int id)
        {
            return await template.QuerySingleAsync(
                "SELECT * FROM Detector d " +
                "JOIN Object o ON (o.id = d.object_id) " +
                "WHERE o.id = @id "
                , MapRowToDetector,
                new QueryParameter("@id", id)
                );
        }

        public async Task InsertAsync(TDetector detector)
        {
            const string SQL_INSERT_OBJECT = "INSERT INTO Object (type) VALUES (@type)";
            const string SQL_INSERT_DETECTOR =
                "INSERT INTO DETECTOR (object_id, client_id, telemetry_name, action_id, check_interval) " +
                "VALUES (@object_id, @client_id, @telemetry_name, @action_id, @check_interval)";
            detector.Id = Convert.ToInt32(
                    await template.ExecuteScalarAsync<object>($"{SQL_INSERT_OBJECT};{LastInsertedIdQuery}",
                    new QueryParameter("@type", detector.GetType().AssemblyQualifiedName)));
            if (detector.Client.Id < 1)
            {
                await clientDao.InsertAsync(detector.Client);
                if (detector.Action?.Client.Id < 1)
                {
                    detector.Action.Client = detector.Client;
                }
            }
            if (detector.Action.Id < 1)
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
            await ObjectLoaderUtilities.InsertProperties<TDetector, Detector<AaaSAction>>(detector.Id, detector, objectPropertyDao);
        }

        public async Task<TDetector> MapRowToDetector(IDataRecord record)
        {
            string typeName = (string)record["type"];
            var type = Type.GetType(typeName);
            var detector = (TDetector)Activator.CreateInstance(type);
            detector.Id = (int)record["id"];
            detector.TelemetryName = (string)record["telemetry_name"];
            detector.CheckInterval = TimeSpan.FromMilliseconds((int)record["check_interval"]);
            detector.Client = await clientDao.FindByIdAsync((int)record["client_id"]);
            detector.Action = await actionDao.FindByIdAsync((int)record["action_id"]);
            await ObjectLoaderUtilities.LoadPropertiesFromId<Detector<TAction>>(detector.Id, detector, objectPropertyDao);
            return detector;
        }

        public async Task<bool> UpdateAsync(TDetector obj)
        {
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            var updated = false;
            if (obj.Client.Id < 1)
            {
                await clientDao.InsertAsync(obj.Client);
            }
            if (obj.Action.Id < 1)
            {
                await actionDao.InsertAsync(obj.Action);
            }
            const string SQL_UPDATE_DETECTOR = "UPDATE Detector SET client_id=@client_id, telemetry_name=@telemetry_name, action_id=@action_id, check_interval=@check_interval WHERE object_id=@object_id";
            var updatedRows = await template.ExecuteAsync(
                SQL_UPDATE_DETECTOR,
                new QueryParameter("@client_id", obj.Client.Id),
                new QueryParameter("@telemetry_name", obj.TelemetryName),
                new QueryParameter("@action_id", obj.Action.Id),
                new QueryParameter("@check_interval", obj.CheckInterval.TotalMilliseconds),
                new QueryParameter("@object_id", obj.Id)
                );
            if (updatedRows <= 0) // updated rows ist auch 1, wenn eine Reihe geupdated wurde, jedoch alle Werte gleich sind
            {
                return false;
            }
            if (await ObjectLoaderUtilities.UpdateProperties<TDetector, Detector<AaaSAction>>(obj.Id, obj, objectPropertyDao) || updatedRows > 0)
            {
                updated = true;
            }
            if (updated)
            {
                scope.Complete();
            }

            return updated;
        }
    }
}
