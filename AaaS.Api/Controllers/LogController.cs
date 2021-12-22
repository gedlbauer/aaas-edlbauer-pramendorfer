using AaaS.Api.Dtos.Telemetry;
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
    public class LogController : TelemetryController<Log, LogDto>
    {
        private readonly ILogRepository _logRepository;
        public LogController(ILogRepository logRepository, IMapper mapper) : base(logRepository, mapper) {
            _logRepository = logRepository;
        }

        [HttpGet("[controller]s/types")]
        public IEnumerable<LogType> GetLogTypes()
            => _logRepository.FindAllLogTypesAsync().ToEnumerable();
        
    }
}
