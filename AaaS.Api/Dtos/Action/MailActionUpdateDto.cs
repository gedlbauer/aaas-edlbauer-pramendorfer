using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Action
{
    public class MailActionUpdateDto : MailActionInsertDto
    {
        public int Id { get; set; }
    }
}
