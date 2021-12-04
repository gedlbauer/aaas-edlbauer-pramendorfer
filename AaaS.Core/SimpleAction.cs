using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core
{
    public class SimpleAction : AaaSAction
    {
        public string Email { get; set; }
        public string TemplateText { get; set; }
        public int Value { get; set; }

        public void Execute()
        {
            Console.WriteLine($"Hello from SimpleAction: {TemplateText} - {Value}");
        }
    }
}
