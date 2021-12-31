using AaaS.Common;
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
    public abstract class AdoMetricDao : AdoTelemetryDao<Metric>, IMetricDao
    {
        protected override string Query => "Select t.Id, t.creation_time, t.Name, t.client_id, t.creator_id, m.value" +
            " from Telemetry as t Inner join[Metric] as m on m.telemetry_id=t.id";

        public AdoMetricDao(IConnectionFactory factory, IClientDao clientDao) : base(factory, clientDao) { }

        public async Task<Metric> FindMostRecentByNameAndClientAsync(int clientId, string name)
            => await template.QuerySingleAsync(
                "Select top 1 t.Id, t.creation_time, t.Name, t.client_id, t.creator_id, m.value from Telemetry as t Inner join[Metric] as m on m.telemetry_id=t.id where name=@name and client_id=@cid order by id desc",
                MapRowToTelemetry,
                new QueryParameter("@name", name),
                new QueryParameter("@cid", clientId));

        protected override async Task InsertDerivationAsync(Metric obj)
        {
            const string SQL_INSERT = "insert into metric (telemetry_id, value) values (@id, @val)";
            await template.ExecuteScalarAsync<object>($"{SQL_INSERT};{LastInsertedIdQuery}",
                 new QueryParameter("@id", obj.Id),
                 new QueryParameter("@val", obj.Value));
        }

        protected override async Task<Metric> MapRowToTelemetry(IDataRecord row)
        {
            Client client = await clientDao.FindByIdAsync((int)row["client_id"]);
            return new Metric
            {
                Id = (int)row["id"],
                Timestamp = (DateTime)row["creation_time"],
                Name = (string)row["name"],
                Client = client,
                CreatorId = Guid.Parse((string)row["creator_id"]),
                Value = (double)row["value"]
            };
        }

        protected override async Task<int> UpdateDerivationAsync(Metric obj)
        {
            const string SQL_UPDATE = "update metric set value=@val where telemetry_id=@id";
            return await template.ExecuteAsync(
                SQL_UPDATE,
                new QueryParameter("@val", obj.Value),
                new QueryParameter("@id", obj.Id));
        }

        public IAsyncEnumerable<string> FindNamesByClientAsync(int clientId)
        {
            const string SQL = "SELECT DISTINCT t.name FROM telemetry t RIGHT JOIN metric m on (t.id=m.telemetry_id) WHERE t.client_id=@cid";
            return template.QueryAsync(SQL,
                x => (string)x["name"],
                new QueryParameter("@cid", clientId)
            );
        }
    }
}
