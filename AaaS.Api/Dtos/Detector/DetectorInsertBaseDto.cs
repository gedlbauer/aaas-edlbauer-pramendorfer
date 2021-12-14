using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public abstract class DetectorInsertBaseDto
    {
        public int ActionId { get; set; }
        public string TelemetryName { get; set; }
        public TimeSpan CheckInterval { get; set; }
    }
}
