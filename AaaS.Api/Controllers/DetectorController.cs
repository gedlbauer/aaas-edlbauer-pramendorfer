using AaaS.Api.Dtos.Detector;
using AaaS.Api.Extensions;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.Extensions;
using AaaS.Core.Managers;
using AaaS.Core.Repositories;
using AaaS.Dal.Interface;
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
        private readonly IMapper _mapper;
        private readonly DetectorManager _detectorManager;
        private readonly ActionManager _actionManager;
        private readonly MetricRepository _metricRepository;
        private readonly IClientDao _clientDao;

        public DetectorController(DetectorManager detectorManager, IMapper mapper, ActionManager actionManager, IClientDao clientDao, MetricRepository metricRepository)
        {
            _detectorManager = detectorManager;
            _mapper = mapper;
            _actionManager = actionManager;
            _clientDao = clientDao;
            _metricRepository = metricRepository;
        }

        [HttpGet]
        public IEnumerable<DetectorDto> GetAll()
        {
            return _mapper.Map<IEnumerable<DetectorDto>>(_detectorManager.GetAll(User.GetId()));
        }

        [HttpGet("{id}")]
        public DetectorDto ById(int id)
        {
            return _mapper.Map<DetectorDto>(_detectorManager.FindDetectorById(User.GetId(), id));
        }

        [HttpPost("averageslidingwindow")]
        public async Task<IActionResult> InsertAverageSlidingWindowDetector(SlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = await MapSlidingWindowDetector<AverageSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPost("currentvalueslidingwindow")]
        public async Task<IActionResult> InsertCurrentValueSlidingWindowDetector(SlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = await MapSlidingWindowDetector<CurrentValueSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPost("sumslidingwindow")]
        public async Task<IActionResult> InsertSumSlidingWindowDetector(SlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = await MapSlidingWindowDetector<SumSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPost("minmax")]
        public async Task<IActionResult> InsertMinMaxDetector(MinMaxDetectorInsertDto detectorDto)
        {
            var clientId = User.GetId();
            var detector = _mapper.Map<MinMaxDetector>(detectorDto);
            detector.MetricRepository = _metricRepository;
            await detector.ResolveNavigationProperties(detectorDto.ActionId, clientId, _actionManager, _clientDao);
            return await InsertBaseDetector(detector);
        }

        private async Task<T> MapSlidingWindowDetector<T>(SlidingWindowDetectorInsertDto detectorDto) where T : BaseDetector
        {
            var clientId = User.GetId();
            var detector = _mapper.Map<T>(detectorDto);
            detector.MetricRepository = _metricRepository;
            await detector.ResolveNavigationProperties(detectorDto.ActionId, clientId, _actionManager, _clientDao);
            return detector;
        }

        private async Task<IActionResult> InsertBaseDetector(BaseDetector detector)
        {
            await _detectorManager.AddAndStartDetectorAsync(detector);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = detector.Id }, value: _mapper.Map<DetectorDto>(detector));
        }

    }
}
