using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Action
{
    public class WebHookActionDto : ActionDto
    {
        public string RequestUrl { get; set; }
    }
}
