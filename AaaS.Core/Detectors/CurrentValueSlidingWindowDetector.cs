using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public class CurrentValueSlidingWindowDetector : SlidingWindowDetector
    {
        public CurrentValueSlidingWindowDetector(IMetricDao metricDao = null) : base(metricDao)
        {
        }

        public async override Task<double> CalculateCheckValue()
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeWindow);
            var metrics = MetricDao.FindSinceByClientAsync(fromDate, Client.Id);
            return (await metrics.OrderBy(x => x.Timestamp).LastOrDefaultAsync())?.Value ?? 0;
        }
    }
}
