﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AaaS.Common;
using AaaS.Dal.Ado.Utilities;
using AaaS.Dal.Interface;
using AaaS.Domain;

namespace AaaS.Dal.Ado.Telemetry
{
    public abstract class AdoTelemetryDao<T> : IBaseDao<T> where T : Domain.Telemetry
    {
        protected readonly AdoTemplate template;
        protected abstract string Query { get; }
        protected abstract string LastInsertedIdQuery { get; }

        public AdoTelemetryDao(IConnectionFactory factory)
        {
            template = new AdoTemplate(factory);
        }

        protected abstract Task<T> MapRowToTelemetry(IDataRecord row);

        public async IAsyncEnumerable<T> FindAllAsync()
        {
            var items = template.QueryAsync(Query, MapRowToTelemetry);

            await foreach (var item in items)
            {
                yield return item;
            }
        }

        public async Task<T> FindByIdAsync(int id)
            => await template.QuerySingleAsync(
                    Query + " where t.id=@id",
                    MapRowToTelemetry,
                    new QueryParameter("@id", id));

        public async Task InsertAsync(T obj)
        {
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
            const string SQL_UPDATE_TELEM = "update telemetry set creation_time=@ct, name=@name, client_id=@cid, creator_id=@creatorid where id=@id";
            int resultTelem = await template.ExecuteAsync(
               SQL_UPDATE_TELEM,
               new QueryParameter("@ct", obj.Timestamp),
               new QueryParameter("@name", obj.Name),
               new QueryParameter("@cid", obj.Id),
               new QueryParameter("@creatorid", obj.CreatorId),
               new QueryParameter("@id", obj.Id));

            return resultTelem == 1 && (await UpdateDerivationAsync(obj)) == 1;
        }

        protected abstract Task<int> UpdateDerivationAsync(T obj);
    }
}
