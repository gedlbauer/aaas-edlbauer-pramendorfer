using AaaS.Common;
using AaaS.Dal.Ado.Utilities;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado.Telemetry
{
    public abstract class AdoLogDao : AdoTelemetryDao<Log>, ILogDao
    {
        protected override string Query => "Select top 1000 t.Id, t.creation_time, t.Name, t.client_id, t.creator_id, " +
            "lt.name as typename, l.message, l.type_id from Telemetry as t Inner join[Log] " +
            "as l on l.telemetry_id=t.id inner join LogType as lt on l.[type_id]=lt.id";

        public AdoLogDao(IConnectionFactory factory, IClientDao clientDao) : base(factory, clientDao) { }

        protected override async Task<Log> MapRowToTelemetry(IDataRecord row)
        {
            LogType type = new LogType { Id = (int)row["type_id"], Name = (string)row["typename"] };
            Client client = await clientDao.FindByIdAsync((int)row["client_id"]);
            return new Log
            {
                Id = (int)row["id"],
                Timestamp = (DateTime)row["creation_time"],
                Name = (string)row["name"],
                Client = client,
                CreatorId = Guid.Parse((string)row["creator_id"]),
                Message = (string)row["message"],
                Type = type
            };
        }

        public IAsyncEnumerable<LogType> FindAllLogTypesAsync()
        {
            const string SQL_GET_LOG_TYPES = "select * from LogType";
            return template.QueryAsync(SQL_GET_LOG_TYPES, MapRowToLogType);
        }

        private LogType MapRowToLogType(IDataRecord row)
            => new LogType { Id = (int)row["id"], Name = (string)row["name"] };

        protected override async Task InsertDerivationAsync(Log log)
        {
            const string SQL_INSERT_LOG = "insert into log (telemetry_id, type_id, message) values (@id, @tid, @message)";
            await template.ExecuteScalarAsync<object>($"{SQL_INSERT_LOG};{LastInsertedIdQuery}",
                 new QueryParameter("@id", log.Id),
                 new QueryParameter("@tid", log.Type.Id),
                 new QueryParameter("@message", log.Message));
        }

        protected override async Task<int> UpdateDerivationAsync(Log log)
        {
            const string SQL_UPDATE_LOG = "update log set type_id=@tid, message=@message where telemetry_id=@id";
            return await template.ExecuteAsync(
                SQL_UPDATE_LOG,
                new QueryParameter("@tid", log.Type.Id),
                new QueryParameter("@message", log.Message),
                new QueryParameter("@id", log.Id));
        }

    }
}
