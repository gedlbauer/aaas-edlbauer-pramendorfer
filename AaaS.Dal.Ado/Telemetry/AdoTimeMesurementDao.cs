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
    public abstract class AdoTimeMesurementDao : AdoTelemetryDao<TimeMeasurement>, ITimeMeasurementDao
    {
        protected override string Query => "Select t.Id, t.creation_time, t.Name, t.client_id, t.creator_id, " +
            "time.start_time, time.end_time from Telemetry as t Inner join[TimeMeasurement] as time on time.telemetry_id=t.id";

        public AdoTimeMesurementDao(IConnectionFactory factory, IClientDao clientDao) : base(factory, clientDao) { }

        protected override async Task InsertDerivationAsync(TimeMeasurement obj)
        {
            const string SQL_INSERT = "insert into TimeMeasurement (telemetry_id, start_time, end_time) values (@id, @sd, @ed)";
            await template.ExecuteScalarAsync<object>($"{SQL_INSERT};{LastInsertedIdQuery}",
                 new QueryParameter("@id", obj.Id),
                 new QueryParameter("@sd", obj.StartTime),
                 new QueryParameter("@ed", obj.EndTime));
        }

        protected override async Task<TimeMeasurement> MapRowToTelemetry(IDataRecord row)
        {
            Client client = await clientDao.FindByIdAsync((int)row["client_id"]);
            return new TimeMeasurement
            {
                Id = (int)row["id"],
                Timestamp = (DateTime)row["creation_time"],
                Name = (string)row["name"],
                Client = client,
                CreatorId = Guid.Parse((string)row["creator_id"]),
                StartTime = (DateTime)row["start_time"],
                EndTime = (DateTime)row["end_time"]
            };
        }

        protected override async Task<int> UpdateDerivationAsync(TimeMeasurement obj)
        {
            const string SQL_UPDATE = "update TimeMeasurement set start_time=@st, end_time=@ed where telemetry_id=@id";
            return await template.ExecuteAsync(
                SQL_UPDATE,
                new QueryParameter("@st", obj.StartTime),
                new QueryParameter("@ed", obj.EndTime),
                new QueryParameter("@id", obj.Id));
        }
    }
}
