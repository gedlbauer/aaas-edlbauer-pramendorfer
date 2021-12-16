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
        private readonly ITelemetryRepository<Metric> _metricRepository;
        private readonly IClientDao _clientDao;

        public DetectorController(DetectorManager detectorManager, IMapper mapper, ActionManager actionManager, IClientDao clientDao, ITelemetryRepository<Metric> metricRepository)
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
            return _mapper.Map<IEnumerable<DetectorDto>>(_detectorManager.GetAllFromClient(User.GetId()));
        }

        [HttpGet("{id}")]
        public DetectorDto ById(int id)
        {
            return _mapper.Map<DetectorDto>(_detectorManager.FindDetectorById(User.GetId(), id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detectorToDelete = _detectorManager.FindDetectorById(id);
            if (detectorToDelete is not null && detectorToDelete.Client.Id != User.GetId())
            {
                return Forbid();
            }
            await _detectorManager.DeleteDetectorAsync(detectorToDelete);
            return NoContent();
        }

        #region AverageSlidingWindowDetector
        [HttpPost("averageslidingwindow")]
        public async Task<IActionResult> InsertAverageSlidingWindowDetector(SlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = await MapSlidingWindowDetectorInsertDto<AverageSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPut("averageslidingwindow")]
        public async Task<IActionResult> UpdateAverageSlidingWindowDetector(SlidingWindowDetectorUpdateDto detectorDto)
        {
            return await UpdateBaseDetector<AverageSlidingWindowDetector>(detectorDto);
        }

        #endregion

        #region CurrentValueSlidingWindowDetector
        [HttpPost("currentvalueslidingwindow")]
        public async Task<IActionResult> InsertCurrentValueSlidingWindowDetector(SlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = await MapSlidingWindowDetectorInsertDto<CurrentValueSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPut("currentvalueslidingwindow")]
        public async Task<IActionResult> UpdateCurrentValueSlidingWindowDetector(SlidingWindowDetectorUpdateDto detectorDto)
        {
            return await UpdateBaseDetector<CurrentValueSlidingWindowDetector>(detectorDto);
        }
        #endregion

        #region SumSlidingWindowDetector
        [HttpPost("sumslidingwindow")]
        public async Task<IActionResult> InsertSumSlidingWindowDetector(SlidingWindowDetectorInsertDto detectorDto)
        {
            var detector = await MapSlidingWindowDetectorInsertDto<SumSlidingWindowDetector>(detectorDto);
            return await InsertBaseDetector(detector);
        }

        [HttpPut("sumslidingwindow")]
        public async Task<IActionResult> UpdateSumSlidingWindowDetector(SlidingWindowDetectorUpdateDto detectorDto)
        {
            return await UpdateBaseDetector<SumSlidingWindowDetector>(detectorDto);
        }
        #endregion

        #region MinMaxDetector
        [HttpPost("minmax")]
        public async Task<IActionResult> InsertMinMaxDetector(MinMaxDetectorInsertDto detectorDto)
        {
            var clientId = User.GetId();
            var detector = _mapper.Map<MinMaxDetector>(detectorDto);
            detector.MetricRepository = _metricRepository;
            await detector.ResolveNavigationProperties(detectorDto.ActionId, clientId, _actionManager, _clientDao);
            return await InsertBaseDetector(detector);
        }

        [HttpPut("minmax")]
        public async Task<IActionResult> UpdateMinMaxDetector(MinMaxDetectorUpdateDto detectorDto)
        {
            var userId = User.GetId();

            var checkError = DoUpdateValidationChecks<MinMaxDetector>(detectorDto.Id);
            if (checkError is not null)
            {
                return checkError;
            }

            var detector = _mapper.Map<MinMaxDetector>(detectorDto);
            detector.MetricRepository = _metricRepository;
            await detector.ResolveNavigationProperties(detectorDto.ActionId, userId, _actionManager, _clientDao);

            await _detectorManager.UpdateDetectorAsync(detector);
            return NoContent();
        }
        #endregion

        private IActionResult DoUpdateValidationChecks<T>(int detectorDtoId)
        {
            var oldDetector = _detectorManager.FindDetectorById(detectorDtoId);
            if (oldDetector is null)
            {
                return NotFound(detectorDtoId);
            }
            else if (oldDetector.Client.Id != User.GetId())
            {
                return Forbid();
            }
            else if (typeof(T) != oldDetector.GetType())
            {
                return Conflict("Type of Detector must not be changed");
            }
            return null;
        }

        private async Task<T> MapSlidingWindowDetectorInsertDto<T>(SlidingWindowDetectorInsertDto detectorDto) where T : BaseDetector
        {
            var clientId = User.GetId();
            var detector = _mapper.Map<T>(detectorDto);
            detector.MetricRepository = _metricRepository;
            await detector.ResolveNavigationProperties(detectorDto.ActionId, clientId, _actionManager, _clientDao);
            return detector;
        }

        private async Task<T> MapSlidingWindowDetectorUpdateDto<T>(SlidingWindowDetectorUpdateDto detectorDto) where T : BaseDetector
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

        private async Task<IActionResult> UpdateBaseDetector<T>(SlidingWindowDetectorUpdateDto detectorDto) where T : BaseDetector
        {
            var checkError = DoUpdateValidationChecks<T>(detectorDto.Id);
            if (checkError is not null)
            {
                return checkError;
            }
            var detector = await MapSlidingWindowDetectorUpdateDto<T>(detectorDto);
            await _detectorManager.UpdateDetectorAsync(detector);
            return NoContent();
        }

    }
}
