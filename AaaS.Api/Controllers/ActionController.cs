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
        private readonly IMapper _mapper;

        public ActionController(ActionManager actionManager, IMapper mapper)
        {
            _actionManager = actionManager;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<IAction> GetAll()
        {
            return _actionManager.GetAll();
        }

        [HttpGet("{id}")]
        public IAction ById(int id)
        {
            return _actionManager.FindActionById(id);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> InsertWebHookAction(WebHookActionInsertDto action)
        {
            var webHookAction = _mapper.Map<WebHookAction>(action);
            await _actionManager.AddActionAsync(webHookAction);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = webHookAction.Id }, value: webHookAction);
        }

        [HttpPost("mail")]
        public async Task<IActionResult> InsertMailAction(MailActionInsertDto action)
        {
            var mailAction = _mapper.Map<MailAction>(action);
            await _actionManager.AddActionAsync(mailAction);
            return CreatedAtAction(actionName: nameof(ById), routeValues: new { id = mailAction.Id }, value: mailAction);
        }
    }
}
