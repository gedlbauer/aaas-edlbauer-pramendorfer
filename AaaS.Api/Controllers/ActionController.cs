using AaaS.Api.Dtos.Action;
using AaaS.Api.Extensions;
using AaaS.Core.Actions;
using AaaS.Core.Extensions;
using AaaS.Core.Managers;
using AaaS.Dal.Interface;
using AaaS.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ActionController : ControllerBase
    {
        private readonly IClientDao _clientDao;
        private readonly IActionManager _actionManager;
        private readonly IDetectorManager _detectorManager;
        private readonly IMapper _mapper;

        public ActionController(IActionManager actionManager, IDetectorManager detectorManager, IClientDao clientDao, IMapper mapper)
        {
            _actionManager = actionManager;
            _detectorManager = detectorManager;
            _mapper = mapper;
            _clientDao = clientDao;
        }

        [HttpGet]
        public IEnumerable<ActionDto> GetAll()
        {
            return _mapper.Map<IEnumerable<ActionDto>>(_actionManager.GetAllFromClient(User.GetId()));
        }

        [HttpGet("{id}")]
        public ActionDto ById(int id)
        {
            return _mapper.Map<ActionDto>(_actionManager.FindActionById(User.GetId(), id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var actionToDelete = _actionManager.FindActionById(id);
            if (actionToDelete is null) return NoContent();
            if (actionToDelete.Client.Id != User.GetId())
            {
                return Forbid();
            }
            if (_detectorManager.GetAll().Any(x => x.Action.Id == id))
            {
                return Conflict(new { message = "There are still detectors that depend on this action" });
            }
            await _actionManager.DeleteActionAsync(actionToDelete);
            return NoContent();
        }

        #region WebHookAction
        [HttpPost("webhook")]
        public async Task<IActionResult> InsertWebHookAction(WebHookActionInsertDto action)
        {
            var webHookAction = _mapper.Map<WebHookAction>(action);
            await webHookAction.ResolveNavigationProperties(User.GetId(), _clientDao);
            await _actionManager.AddActionAsync(webHookAction);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = webHookAction.Id }, value: _mapper.Map<WebHookActionDto>(webHookAction));
        }

        [HttpPut("webhook")]
        public async Task<IActionResult> UpdateWebHookAction(WebHookActionUpdateDto actionDto)
        {
            var checkError = DoUpdateValidationChecks<WebHookAction>(actionDto.Id);
            if (checkError is not null)
            {
                return checkError;
            }
            var mailAction = _mapper.Map<WebHookAction>(actionDto);
            await mailAction.ResolveNavigationProperties(User.GetId(), _clientDao);
            await _actionManager.UpdateActionAsync(mailAction);
            return NoContent();
        }
        #endregion

        #region MailAction
        [HttpPost("mail")]
        public async Task<IActionResult> InsertMailAction(MailActionInsertDto action)
        {
            var mailAction = _mapper.Map<MailAction>(action);
            await mailAction.ResolveNavigationProperties(User.GetId(), _clientDao);
            await _actionManager.AddActionAsync(mailAction);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = mailAction.Id }, value: _mapper.Map<MailActionDto>(mailAction));
        }

        [HttpPut("mail")]
        public async Task<IActionResult> UpdateMailAction(MailActionUpdateDto actionDto)
        {
            var checkError = DoUpdateValidationChecks<MailAction>(actionDto.Id);
            if (checkError is not null)
            {
                return checkError;
            }
            var mailAction = _mapper.Map<MailAction>(actionDto);
            await mailAction.ResolveNavigationProperties(User.GetId(), _clientDao);
            await _actionManager.UpdateActionAsync(mailAction);
            return NoContent();
        }
        #endregion

        private IActionResult DoUpdateValidationChecks<T>(int actionDtoId)
        {
            var oldAction = _actionManager.FindActionById(actionDtoId);
            if (oldAction is null)
            {
                return NotFound(actionDtoId);
            }
            else if (oldAction.Client.Id != User.GetId())
            {
                return Forbid();
            }
            else if (typeof(T) != oldAction.GetType())
            {
                return Conflict("Type of Action must not be changed");
            }
            return null;
        }
    }
}
