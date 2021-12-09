using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public class SumSlidingWindowDetector : SlidingWindowDetector
    {
        public SumSlidingWindowDetector(IMetricDao metricDao = null) : base(metricDao) { }

        public override Task<double> CalculateCheckValue()
        {
            throw new NotImplementedException();
        }
    }
}
