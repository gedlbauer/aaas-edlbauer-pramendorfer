using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Action
{
    public class WebHookActionUpdateDto : WebHookActionInsertDto
    {
        public int Id { get; set; }
    }
}
