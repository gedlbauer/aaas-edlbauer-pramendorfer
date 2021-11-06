using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public class MinMaxDetector : Detector
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Threshold { get; set; }

    }
}
