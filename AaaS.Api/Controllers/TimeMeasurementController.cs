using AaaS.Api.Dtos.Telemetry;
using AaaS.Core.Repositories;
using AaaS.Domain;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Controllers
{
    public class TimeMeasurementController : TelemetryController<TimeMeasurement, TimeMeasurementDto, TimeMeasurementDto>
    {
        public TimeMeasurementController(TimeMeasurementRepository repo, IMapper mapper) : base(repo, mapper)
        {

        }
    }
}
