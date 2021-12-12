using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Telemetry
{
    public class TimeMeasurementDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
