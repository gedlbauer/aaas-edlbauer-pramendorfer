using AaaS.Api.Dtos.Detector;
using AaaS.Core.Actions;
using AaaS.Core.Managers;
using AaaS.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectorController : ControllerBase
    {
        private readonly DetectorManager _detectorManager;
        private readonly IMapper _mapper;

        public DetectorController(DetectorManager detectorManager, IMapper mapper)
        {
            _detectorManager = detectorManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<DetectorDto> GetAll()
        {
            return _mapper.Map<IEnumerable<DetectorDto>>(_detectorManager.GetAll());
        }

        [HttpGet("{id}")]
        public DetectorDto ById(int id)
        {
            return _mapper.Map<DetectorDto>(_detectorManager.FindDetectorById(id));
        }
    }
}
