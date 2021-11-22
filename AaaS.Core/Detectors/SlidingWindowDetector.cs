using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public abstract class SlidingWindowDetector : Detector
    {
        public TimeSpan TimeWindow { get; set; }
        public bool UseGreater { get; set; }
        public int Threshold { get; set; }

        public abstract double CalculateCheckValue();

        protected override void Detect()
        {
            var value = CalculateCheckValue();
            if ((UseGreater && value > Threshold) || value < Threshold)
            {
                Action.Execute();
            }
        }
    }
}
