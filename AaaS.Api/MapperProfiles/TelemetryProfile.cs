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
            CreateMap<Log, LogDto>().ReverseMap();
            CreateMap<Metric, MetricDto>().ReverseMap();
            CreateMap<TimeMeasurement, TimeMeasurementDto>().ReverseMap();

            CreateMap<CounterInsertDto, Metric>();
            CreateMap<MeasurementInsertDto, Metric>();
            CreateMap<TimeMeasurementInsertDto, TimeMeasurement>();
            CreateMap<LogInsertDto, Log>()
                .AfterMap((src, dest) =>
                {
                    dest.Type = new LogType() { Id = src.LogTypeId };
                });
        }
    }
}
