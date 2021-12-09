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

        public async override Task<double> CalculateCheckValue()
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeWindow);
            return await MetricDao.FindSinceByClientAsync(fromDate, Client.Id).SumAsync(x => x.Value);
        }
    }
}
