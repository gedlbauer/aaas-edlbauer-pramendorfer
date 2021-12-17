using AaaS.Api.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public class SlidingWindowDetectorInsertDto : DetectorInsertBaseDto
    {
        [MinDuration(1000)]
        public TimeSpan TimeWindow { get; set; }
        public bool UseGreater { get; set; }
        public int Threshold { get; set; }
    }
}
