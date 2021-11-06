using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public class Log : Telemetry
    {
        public Type Type { get; set; }
        public string Message { get; set; }
    }
}
