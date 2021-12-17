using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        public IActionResult PutHeartbeat(Guid creatorId)
        {
            Console.WriteLine($"Heartbeat from {creatorId}");
            return Ok();
        }

        [HttpPost("logoff")]
        public IActionResult LogOff(Guid creatorId)
        {
            Console.WriteLine($"{creatorId} logged off");
            return Ok();
        }
    }
}
