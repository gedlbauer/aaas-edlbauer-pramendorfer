using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public class MinMaxDetectorDto : DetectorDto
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxOccurs { get; set; }
        public TimeSpan TimeWindow { get; set; }
    }
}
