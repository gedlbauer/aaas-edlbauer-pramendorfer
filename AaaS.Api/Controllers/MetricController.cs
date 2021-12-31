using AaaS.Api.Dtos.Telemetry;
using AaaS.Api.Extensions;
using AaaS.Core.Repositories;
using AaaS.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class MetricController : TelemetryController<Metric, MetricDto>
    {
        private readonly IMetricRepository _metricRepository;
        public MetricController(IMetricRepository metricRepository, IMapper mapper) : base(metricRepository, mapper)
        {
            _metricRepository = metricRepository;
        }

        [HttpGet("[controller]s/names")]
        public IEnumerable<string> GetMetricNames()
            => _metricRepository.FindAllMetricNamesFromClientAsync(User.GetId()).ToEnumerable();
    }
}
