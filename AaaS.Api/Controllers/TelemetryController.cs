using AaaS.Api.Extensions;
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

    public abstract class TelemetryController<T, TRead> : ControllerBase where T : Telemetry
    {
        protected readonly ITelemetryRepository<T> _telemetryRepository;
        protected readonly IMapper _mapper;

        public TelemetryController(ITelemetryRepository<T> telemetryRepository, IMapper mapper)
        {
            _telemetryRepository = telemetryRepository;
            _mapper = mapper;
        }

        [HttpGet("[controller]s/{id}")]
        public async Task<ActionResult<TRead>> GetTelemetry(int id)
        {
          
            var telemetry = await _telemetryRepository.FindByIdAsync(User.GetId(), id);
            return telemetry == null ? NotFound() : _mapper.Map<TRead>(telemetry);
        }

        [HttpGet("[controller]s")]
        public IEnumerable<TRead> GetTelemetries(string name, int? amount)
        {
            IEnumerable<T> telemetries;
            if (name != null)
                telemetries = _telemetryRepository.FindByAllByNameAsync(User.GetId(), name).ToEnumerable();
            else
                telemetries = _telemetryRepository.FindAllAsync(User.GetId()).ToEnumerable();
            if(amount is not null)
            {
                telemetries = telemetries.Take(amount.Value).OrderBy(x => x.Timestamp);
            }
            return _mapper.Map<IEnumerable<TRead>>(telemetries);
        }

        [HttpGet("creators/creatorId/[controller]s")]
        public IEnumerable<TRead> GetTelemetriesByCreator(Guid creatorId)
        {
            IEnumerable<T> telemetries = _telemetryRepository.FindByCreatorAsync(User.GetId(), creatorId).ToEnumerable();
            return _mapper.Map<IEnumerable<TRead>>(telemetries);
        }
    }
}
