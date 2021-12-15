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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TelemetryCommandController : ControllerBase
    {
        private readonly ITelemetryRepository<Metric> _metricRepository;
        private readonly IMapper _mapper;

        public TelemetryCommandController(ITelemetryRepository<Metric> metricRepository, IMapper mapper)
        {
            _mapper = mapper;
            _metricRepository = metricRepository;
        }

        [HttpPut("metric")]
        public async Task<ActionResult<MetricDto>> CreateMetric(MetricInsertDto metricDto)
        {
            Metric metric = _mapper.Map<Metric>(metricDto);
            metric.Client.Id = User.GetId();
            await _metricRepository.InsertAsync(metric);
            return CreatedAtAction(
                nameof(MetricController.GetTelemetry),
                new { id = metric.Id },
                _mapper.Map<MetricDto>(metric));
        }
    }
}
