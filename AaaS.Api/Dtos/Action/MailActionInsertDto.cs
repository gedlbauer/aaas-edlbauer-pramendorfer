using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Dtos.Action
{
    public class MailActionInsertDto
    {
        public string Name { get; set; }
        public string MailAddress { get; set; }
        public string MailContent { get; set; }
    }
}
