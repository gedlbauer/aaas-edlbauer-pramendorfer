using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public class MinMaxDetector : Detector
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxOccurs { get; set; }
        public TimeSpan TimeWindow {get; set;}

        public override void Detect()
        {
            throw new NotImplementedException();
        }
    }
}
