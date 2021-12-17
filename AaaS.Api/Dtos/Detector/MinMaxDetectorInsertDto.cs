using AaaS.Api.Attributes;
using AaaS.Api.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public class MinMaxDetectorInsertDto : DetectorInsertBaseDto
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxOccurs { get; set; }

        [MinDuration(1000)]
        public TimeSpan TimeWindow { get; set; }
    }
}
