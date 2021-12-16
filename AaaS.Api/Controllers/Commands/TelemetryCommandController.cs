using AaaS.Api.Dtos.Telemetry;
using AaaS.Api.Extensions;
using AaaS.Core.Repositories;
using AaaS.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Controllers.Commands
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TelemetryCommandController : ControllerBase
    {
        private readonly IMetricRepository _metricRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITelemetryRepository<TimeMeasurement> _timeMeasurementRepository;
        private readonly IMapper _mapper;

        public TelemetryCommandController(IMetricRepository metricRepository, ILogRepository logRepository, ITelemetryRepository<TimeMeasurement> timeMeasurementRepository, IMapper mapper)
        {
            _mapper = mapper;
            _metricRepository = metricRepository;
            _logRepository = logRepository;
            _timeMeasurementRepository = timeMeasurementRepository;
        }

        [HttpPut("counters")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCounter(CounterInsertDto metricDto)
        {
            Metric metric = _mapper.Map<Metric>(metricDto);
            metric.Client = new Client { Id = User.GetId() };
            await _metricRepository.InsertCounterAsync(metric);
            return CreatedAtAction(
                nameof(MetricController.GetTelemetry),
                "Metric",
                new { id = metric.Id },
                _mapper.Map<MetricDto>(metric));
        }

        [HttpPut("measurements")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMeasurement(MeasurementInsertDto metricDto)
        {
            Metric metric = _mapper.Map<Metric>(metricDto);
            metric.Client = new Client { Id = User.GetId() };
            await _metricRepository.InsertMeasurementAsync(metric);
            return CreatedAtAction(
                nameof(MetricController.GetTelemetry),
                "Metric",
                new { id = metric.Id },
                _mapper.Map<MetricDto>(metric));
        }

        [HttpPut("logs")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLog(LogInsertDto logDto)
        {
            Log log = _mapper.Map<Log>(logDto);
            log.Client = new Client { Id = User.GetId() };
            await _logRepository.InsertAsync(log);
            return CreatedAtAction(
                nameof(LogController.GetTelemetry),
                "Log",
                new { id = log.Id },
                _mapper.Map<LogDto>(log));
        }

        [HttpGet("[controller]s/logs/types")]
        public IEnumerable<LogType> GetLogTypes()
         => _logRepository.FindAllLogTypesAsync().ToEnumerable();

        [HttpPut("time-measurements")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTimeMeasurements(TimeMeasurementInsertDto measurementDto)
        {
            TimeMeasurement measurement = _mapper.Map<TimeMeasurement>(measurementDto);
            measurement.Client = new Client { Id = User.GetId() };
            await _timeMeasurementRepository.InsertAsync(measurement);
            return CreatedAtAction(
                nameof(TimeMeasurementController.GetTelemetry),
                "TimeMeasurement",
                new { id = measurement.Id },
                _mapper.Map<TimeMeasurementDto>(measurement));
        }


    }
}
