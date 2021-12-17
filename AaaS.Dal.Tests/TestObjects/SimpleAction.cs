using AaaS.Core.Actions;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Tests.TestObjects
{
    public class SimpleAction : BaseAction
    {
        public string Email { get; set; }
        public string TemplateText { get; set; }
        public int Value { get; set; }

        public override Task Execute()
        {
            throw new NotImplementedException();
        }
    }
}
