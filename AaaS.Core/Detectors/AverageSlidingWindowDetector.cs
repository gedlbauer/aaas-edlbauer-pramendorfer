using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public class AverageSlidingWindowDetector : SlidingWindowDetector
    {
        public AverageSlidingWindowDetector(IMetricDao metricDao = null) : base(metricDao) { }

        public async override Task<double> CalculateCheckValue()
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeWindow);
            return await MetricDao.FindSinceByClientAsync(fromDate, Client.Id).AverageAsync(x => x.Value);
        }
    }
}
