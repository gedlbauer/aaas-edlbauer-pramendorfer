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
    public class MinMaxDetector : BaseDetector
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int MaxOccurs { get; set; }
        public TimeSpan TimeWindow { get; set; }

        public MinMaxDetector(IMetricRepository metricRepository) : base(metricRepository) { }
        public MinMaxDetector() : base(null) { }

        protected async override Task Detect()
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeWindow);
            var metrics = MetricRepository.FindSinceByClientAndTelemetryNameAsync(fromDate, Client.Id, TelemetryName);
            if (await metrics.CountAsync(x => x.Value < Min || x.Value > Max) > MaxOccurs)
            {
                await Action.Execute();
            }
        }
    }
}
