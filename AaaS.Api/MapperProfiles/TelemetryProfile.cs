using AaaS.Api.Dtos.Telemetry;
using AaaS.Domain;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.MapperProfiles
{
    public class TelemetryProfile : Profile
    {
        public TelemetryProfile()
        {
            CreateMap<Log, LogDto>();
            CreateMap<Metric, MetricDto>().ReverseMap();
            CreateMap<TimeMeasurement, TimeMeasurementDto>();

            CreateMap<MetricInsertDto, Metric>();
        }
    }
}
