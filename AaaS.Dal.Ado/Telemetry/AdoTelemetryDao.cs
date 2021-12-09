﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AaaS.Common;
using AaaS.Dal.Ado.Utilities;
using AaaS.Dal.Interface;
using AaaS.Domain;

namespace AaaS.Dal.Ado.Telemetry
{
    public abstract class AdoTelemetryDao<T> : IBaseDao<T> where T : Domain.Telemetry
    {
        protected readonly AdoTemplate template;
        protected readonly IClientDao clientDao;
        protected abstract string Query { get; }
        protected abstract string LastInsertedIdQuery { get; }

        public AdoTelemetryDao(IConnectionFactory factory, IClientDao clientDao)
        {
            template = new AdoTemplate(factory);
            this.clientDao = clientDao;
        }

        protected abstract Task<T> MapRowToTelemetry(IDataRecord row);

        public IAsyncEnumerable<T> FindAllAsync()
            => template.QueryAsync(Query, MapRowToTelemetry);

        public IAsyncEnumerable<T> FindSinceByClientAsync(DateTime dateTime, int clientId)
            => template.QueryAsync(
                Query + " where creation_time=@ct and client_id=@clientId;",
                MapRowToTelemetry,
                new QueryParameter("@ct", dateTime),
                new QueryParameter("@clientId", clientId));

        public async Task<T> FindByIdAsync(int id)
            => await template.QuerySingleAsync(
                    Query + " where t.id=@id",
                    MapRowToTelemetry,
                    new QueryParameter("@id", id));

        public async Task InsertAsync(T obj)
        {
            if (obj.Client.Id < 1)
                await clientDao.InsertAsync(obj.Client);

            const string SQL_INSERT_TELEM = "insert into telemetry (creation_time, name, client_id, creator_id) values (@cd, @name, @cid, @creaid)";
            obj.Id = Convert.ToInt32(await template.ExecuteScalarAsync<object>($"{SQL_INSERT_TELEM};{LastInsertedIdQuery}",
                 new QueryParameter("@name", obj.Name),
                 new QueryParameter("@cd", obj.Timestamp),
                 new QueryParameter("@cid", obj.Client.Id),
                 new QueryParameter("@creaid", obj.CreatorId)));
            await InsertDerivationAsync(obj);
        }

        protected abstract Task InsertDerivationAsync(T obj);

        public async Task<bool> UpdateAsync(T obj)
        {
            if (obj.Client.Id > 0)
            {
                bool success = await clientDao.UpdateAsync(obj.Client);
                if (!success) return false;
            }
            else
                await clientDao.InsertAsync(obj.Client);

            const string SQL_UPDATE_TELEM = "update telemetry set creation_time=@ct, name=@name, client_id=@cid, creator_id=@creatorid where id=@id";
            int resultTelem = await template.ExecuteAsync(
               SQL_UPDATE_TELEM,
               new QueryParameter("@ct", obj.Timestamp),
               new QueryParameter("@name", obj.Name),
               new QueryParameter("@cid", obj.Client.Id),
               new QueryParameter("@creatorid", obj.CreatorId),
               new QueryParameter("@id", obj.Id));

            return resultTelem == 1 && (await UpdateDerivationAsync(obj)) == 1;
        }

        protected abstract Task<int> UpdateDerivationAsync(T obj);

        public async Task<bool> DeleteAsync(T obj)
        {
            int result = await template.ExecuteAsync("delete from telemetry where id=@id",
                 new QueryParameter("@id", obj.Id));
            return result == 1;
        }
    }
}
