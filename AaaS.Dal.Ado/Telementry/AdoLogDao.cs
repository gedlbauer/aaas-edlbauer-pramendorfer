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

namespace AaaS.Dal.Ado.Telementry
{
    public abstract class AdoLogDao : ILogDao
    {
        private const string BASE_SELECT_QUERY = "Select t.Id, t.creation_time, t.Name," +
            "t.client_id, t.creator_id, lt.name as typename, l.message, l.type_id from Telemetry as t Inner join[Log] " +
            "as l on l.telemetry_id=t.id inner join LogType as lt on l.[type_id]=lt.id";
        private readonly AdoTemplate template;
        private readonly IClientDao clientDao;
        protected abstract string LastInsertedIdQuery { get; }

        public AdoLogDao(IConnectionFactory factory, IClientDao clientDao)
        {
            template = new AdoTemplate(factory);
            this.clientDao = clientDao;

        }

        private async Task<Log> MapRowToLog(IDataRecord row)
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

        public async IAsyncEnumerable<Log> FindAllAsync()
        {
            var logs = template.QueryAsync(BASE_SELECT_QUERY, MapRowToLog);

            await foreach (var log in logs)
            {
                yield return log;
            }
        }

        public async Task<Log> FindByIdAsync(int id)
          => await template.QuerySingleAsync(
                BASE_SELECT_QUERY + " where t.id=@id",
                MapRowToLog,
                new QueryParameter("@id", id));

        public async Task InsertAsync(Log log)
        {
            const string SQL_INSERT_TELEM = "insert into telemetry (creation_time, name, client_id, creator_id) values (@cd, @name, @cid, @creaid)";
            const string SQL_INSERT_LOG = "insert into log (telemetry_id, type_id, message) values (@id, @tid, @message)";
            log.Id = Convert.ToInt32(await template.ExecuteScalarAsync<object>($"{SQL_INSERT_TELEM};{LastInsertedIdQuery}",
                 new QueryParameter("@name", log.Name),
                 new QueryParameter("@cd", log.Timestamp),
                 new QueryParameter("@cid", log.Client.Id),
                 new QueryParameter("@creaid", log.CreatorId)));

            await template.ExecuteScalarAsync<object>($"{SQL_INSERT_LOG};{LastInsertedIdQuery}",
                 new QueryParameter("@id", log.Id),
                 new QueryParameter("@tid", log.Type.Id),
                 new QueryParameter("@message", log.Message));
        }

        public Task<bool> UpdateAsync(Log client)
        {
            throw new NotImplementedException();
        }
    }
}
