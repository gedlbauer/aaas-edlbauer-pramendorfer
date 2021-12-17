using AaaS.Core.Repositories;
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
        public CurrentValueSlidingWindowDetector(IMetricRepository metricRepository) : base(metricRepository) { }
        public CurrentValueSlidingWindowDetector() : base(null) { }

        protected async override Task<double> CalculateCheckValue()
        {
            var metrics = MetricRepository.FindSinceByClientAsync(FromDate, Client.Id)
                .Where(x => x.Name == TelemetryName); // TODO Eventuell nach Repo verschieben
            return (await metrics.OrderBy(x => x.Timestamp).LastOrDefaultAsync())?.Value ?? 0;
        }
    }
}
