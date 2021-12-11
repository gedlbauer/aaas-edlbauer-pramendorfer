using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public class MinMaxDetectorInsertDto
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxOccurs { get; set; }
        public TimeSpan TimeWindow { get; set; }
        public int ClientId { get; set; }
        public int ActionId { get; set; }
        public string TelemetryName { get; set; }
        public TimeSpan CheckInterval { get; set; }
    }
}
