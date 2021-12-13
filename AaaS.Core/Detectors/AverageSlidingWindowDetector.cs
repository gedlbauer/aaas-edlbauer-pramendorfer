using AaaS.Core.Repositories;
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
        public AverageSlidingWindowDetector(MetricRepository metricRepository) : base(metricRepository) { }
        public AverageSlidingWindowDetector() : base(null) { }

        public async override Task<double> CalculateCheckValue()
        {
            return await MetricRepository.FindSinceByClientAsync(FromDate, Client.Id).AverageAsync(x => x.Value);
        }
    }
}
