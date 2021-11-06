using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public class SlidingWindowDetector : Detector
    {
        public TimeSpan Threshold { get; set; }
        public bool UseGreater { get; set; }
        // TODO Type
    }
}
