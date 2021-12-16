using AaaS.Api.Dtos.Action;
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
    public class ActionController : ControllerBase
    {
        private readonly ActionManager _actionManager;
        private readonly DetectorManager _detectorManager;
        private readonly IMapper _mapper;

        public ActionController(ActionManager actionManager, DetectorManager detectorManager, IMapper mapper)
        {
            _actionManager = actionManager;
            _detectorManager = detectorManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<AaaSAction> GetAll()
        {
            return _actionManager.GetAll();
        }

        [HttpGet("{id}")]
        public AaaSAction ById(int id)
        {
            return _actionManager.FindActionById(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAction(int id)
        {
            var actionToDelete = _actionManager.FindActionById(id);
            if (actionToDelete is null) return NoContent();
            //if (actionToDelete.Client.Id != User.GetId())
            //{
            //    return Forbid();
            //}
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
            await _actionManager.AddActionAsync(webHookAction);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = webHookAction.Id }, value: webHookAction);
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
            await _actionManager.UpdateActionAsync(mailAction);
            return NoContent();
        }
        #endregion

        #region MailAction
        [HttpPost("mail")]
        public async Task<IActionResult> InsertMailAction(MailActionInsertDto action)
        {
            var mailAction = _mapper.Map<MailAction>(action);
            await _actionManager.AddActionAsync(mailAction);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = mailAction.Id }, value: mailAction);
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
            //else if (oldAction.Client.Id != User.GetId())
            //{
            //    return Forbid();
            //}
            else if (typeof(T) != oldAction.GetType())
            {
                return Conflict("Type of Action must not be changed");
            }
            return null;
        }
    }
}
