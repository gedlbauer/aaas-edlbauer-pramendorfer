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
        public SumSlidingWindowDetector(IMetricDao metricDao) : base(metricDao) { }
        public SumSlidingWindowDetector() : base(null) { }

        public async override Task<double> CalculateCheckValue()
        {
            return await MetricDao.FindSinceByClientAsync(FromDate, Client.Id).SumAsync(x => x.Value);
        }
    }
}
