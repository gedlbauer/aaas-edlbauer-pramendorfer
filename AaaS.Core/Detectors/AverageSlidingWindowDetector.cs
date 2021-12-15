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
        public AverageSlidingWindowDetector(ITelemetryRepository<Metric> metricRepository) : base(metricRepository) { }
        public AverageSlidingWindowDetector() : base(null) { }

        public async override Task<double> CalculateCheckValue()
        {
            var items = MetricRepository.FindSinceByClientAsync(FromDate, Client.Id);
            return await items.AnyAsync() ? await items.AverageAsync(x => x.Value) : 0;
        }
    }
}
