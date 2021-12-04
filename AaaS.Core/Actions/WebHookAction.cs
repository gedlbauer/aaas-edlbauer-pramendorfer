using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Actions
{
    public class WebHookAction : BaseAction
    {
        public string RequestUrl { get; set; }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
