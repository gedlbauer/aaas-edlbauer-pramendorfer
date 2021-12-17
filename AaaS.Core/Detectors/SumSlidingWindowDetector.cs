using AaaS.Core.Repositories;
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
        public SumSlidingWindowDetector(IMetricRepository metricRepository) : base(metricRepository) { }
        public SumSlidingWindowDetector() : base(null) { }

        protected async override Task<double> CalculateCheckValue()
        {
            return await MetricRepository.FindSinceByClientAndTelemetryNameAsync(FromDate, Client.Id, TelemetryName)
                .SumAsync(x => x.Value);
        }
    }
}
