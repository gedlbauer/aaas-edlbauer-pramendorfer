using AaaS.Api.Dtos.Detector;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
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

        [HttpPost("averageslidingwindow")]
        public async Task<IActionResult> InsertAverageSlidingWindowDetector(AverageSlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = _mapper.Map<AverageSlidingWindowDetector>(detectorDto);
            // TODO: mapper für DetectorInsertDtos implementieren (ClientId => Client + ActionId => Action)
            return await InsertBaseDetector(detector);
        }

        [HttpPost("currentvalueslidingwindow")]
        public async Task<IActionResult> InsertCurrentValueSlidingWindowDetector(CurrentValueSlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = _mapper.Map<CurrentValueSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPost("sumslidingwindow")]
        public async Task<IActionResult> InsertSumSlidingWindowDetector(SumSlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = _mapper.Map<SumSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPost("minmax")]
        public async Task<IActionResult> InsertMinMaxDetector(MinMaxDetectorInsertDto detectorDto)
        {
            var detector = _mapper.Map<MinMaxDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        private async Task<IActionResult> InsertBaseDetector(BaseDetector detector)
        {
            await _detectorManager.AddAndStartDetectorAsync(detector);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = detector.Id }, value: _mapper.Map<DetectorDto>(detector));
        }

    }
}
