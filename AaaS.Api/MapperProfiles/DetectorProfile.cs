﻿using AaaS.Api.Dtos.Detector;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Dal.Interface;
using AaaS.Domain;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.MapperProfiles
{
    public class DetectorProfile : Profile
    {
        public DetectorProfile()
        {
            CreateMap<Detector<BaseAction>, DetectorDto>();
            CreateMap<MinMaxDetectorInsertDto, MinMaxDetector>();
            CreateMap<SlidingWindowDetectorInsertDto, SumSlidingWindowDetector>();
            CreateMap<SlidingWindowDetectorInsertDto, AverageSlidingWindowDetector>();
            CreateMap<SlidingWindowDetectorInsertDto, CurrentValueSlidingWindowDetector>();
        }
    }
}