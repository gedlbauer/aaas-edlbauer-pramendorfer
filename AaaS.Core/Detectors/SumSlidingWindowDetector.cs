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
            return await MetricRepository.FindSinceByClientAsync(FromDate, Client.Id)
                .Where(x => x.Name == TelemetryName) // TODO Eventuell nach Repo verschieben
                .SumAsync(x => x.Value);
        }
    }
}
