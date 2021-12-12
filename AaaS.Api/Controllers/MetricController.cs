using AaaS.Api.Dtos.Telemetry;
using AaaS.Core.Repositories;
using AaaS.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class MetricController : TelemetryController<Metric, Metric, MetricDto>
    {
        public MetricController(MetricRepository metricRepository, IMapper mapper) : base(metricRepository, mapper)
        {

        }
    }
}
