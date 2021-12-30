using AaaS.Api.Dtos.Action;
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
    public abstract class DetectorDto
    {
        public int Id { get; set; }
        public ClientDto Client { get; set; }
        public ActionDto Action { get; set; }
        public string TelemetryName { get; set; }
        public TimeSpan CheckInterval { get; set; }
        public bool IsRunning { get; set; }
        public string TypeName { get; set; }
    }
}
