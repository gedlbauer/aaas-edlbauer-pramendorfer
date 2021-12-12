using AaaS.Api.Settings;
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

    public abstract class TelemetryController<T, TWrite, TRead> : ControllerBase where T : Telemetry
    {
        private readonly ITelemetryRepository<T> _telemetryRepository;
        private readonly IMapper _mapper;

        public TelemetryController(ITelemetryRepository<T> telemetryRepository, IMapper mapper)
        {
            _telemetryRepository = telemetryRepository;
            _mapper = mapper;
        }

        [HttpGet("[controller]s/{id}")]
        public async Task<ActionResult<TRead>> GetTelemetry(
            [FromHeader(Name = ApiKeyConstants.HeaderName)] string apiKey,
            int id)
        {
            var telemetry = await _telemetryRepository.FindByIdAsync(apiKey, id);
            return telemetry == null ? NotFound() : _mapper.Map<TRead>(telemetry);
        }

        [HttpGet("[controller]s")]
        public IEnumerable<TRead> GetTelemetries(
            [FromHeader(Name = ApiKeyConstants.HeaderName)] string apiKey,
            string name)
        {
            IEnumerable<T> telemetries;
            if (name != null)
                telemetries = _telemetryRepository.FindByAllByNameAsync(apiKey, name).ToEnumerable();
            else
                telemetries = _telemetryRepository.FindAllAsync(apiKey).ToEnumerable();

            return _mapper.Map<IEnumerable<TRead>>(telemetries);
        }

        [HttpGet("creators/creatorId/[controller]s")]
        public IEnumerable<TRead> GetTelemetriesByCreator(
            [FromHeader(Name = ApiKeyConstants.HeaderName)] string apiKey,
            Guid creatorId)
        {
            IEnumerable<T> telemetries = _telemetryRepository.FindByCreatorAsync(apiKey, creatorId).ToEnumerable();
            return _mapper.Map<IEnumerable<TRead>>(telemetries);
        }
    }
}
