using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public abstract class SlidingWindowDetector : BaseDetector
    {
        public TimeSpan TimeWindow { get; set; }
        public bool UseGreater { get; set; }
        public int Threshold { get; set; }

        protected SlidingWindowDetector(IMetricDao metricDao) : base(metricDao) { }

        public abstract Task<double> CalculateCheckValue();

        protected async override Task Detect()
        {
            var value = await CalculateCheckValue();
            if ((UseGreater && value > Threshold) || value < Threshold)
            {
                await Action.Execute();
            }
        }
    }
}
