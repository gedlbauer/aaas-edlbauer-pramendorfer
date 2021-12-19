using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Options
{
    public class HeartbeatOptions
    {
        public const string Position = "Heartbeats";

        public int Interval { get; set; } = 30_000;
        public int Threshold { get; set; } = 30_000;
        public string WarningEMailAddress { get; set; }

    }
}
