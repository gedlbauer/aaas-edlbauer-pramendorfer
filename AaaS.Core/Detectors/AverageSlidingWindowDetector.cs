using AaaS.Core.Repositories;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public class AverageSlidingWindowDetector : SlidingWindowDetector
    {
        public AverageSlidingWindowDetector(IMetricRepository metricRepository) : base(metricRepository) { }
        public AverageSlidingWindowDetector() : base(null) { }

        protected async override Task<double> CalculateCheckValue()
        {
            var items = MetricRepository.FindSinceByClientAndTelemetryNameAsync(FromDate, Client.Id, TelemetryName);
            return await items.AnyAsync() ? await items.AverageAsync(x => x.Value) : 0;
        }
    }
}
