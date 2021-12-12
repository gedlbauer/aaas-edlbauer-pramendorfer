using AaaS.Api.Dtos.Client;
using AaaS.Api.JsonConverters;
using AaaS.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Detector
{
    public class DetectorDto
    {
        public int Id { get; set; }
        public ClientDto Client { get; set; }
        public AaaSAction Action { get; set; }
        public string TelemetryName { get; set; }

        [JsonConverter(typeof(TimeSpanToMillisecondsConverter))]
        public TimeSpan CheckInterval { get; set; }
    }
}
