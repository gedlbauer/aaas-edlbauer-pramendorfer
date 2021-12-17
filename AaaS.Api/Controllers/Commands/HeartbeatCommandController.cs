using AaaS.Core.HostedServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Controllers.Commands
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HeartbeatCommandController : ControllerBase
    {
        private readonly HeartbeatService _hearbeatService;

        public HeartbeatCommandController(HeartbeatService heartbeatService)
        {
            _hearbeatService = heartbeatService;
        }

        [HttpPost]
        public IActionResult PutHeartbeat(Guid creatorId)
        {
            _hearbeatService.AddHeartbeat(creatorId);
            return Ok();
        }

        [HttpPost("logoff")]
        public IActionResult LogOff(Guid creatorId)
        {
            _hearbeatService.LogOffClient(creatorId);
            return Ok();
        }
    }
}
