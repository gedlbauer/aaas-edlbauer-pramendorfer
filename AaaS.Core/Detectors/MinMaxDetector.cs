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

        public MinMaxDetector(IMetricDao metricDao = null) : base(metricDao) { }

        protected async override Task Detect()
        {
            var fromDate = DateTime.UtcNow.Subtract(TimeWindow);
            var metrics = MetricDao.FindSinceByClientAsync(fromDate, Client.Id);
            if (await metrics.CountAsync(x => x.Value < Min || x.Value > Max) > MaxOccurs)
            {
                await Action.Execute();
            }
        }
    }
}
