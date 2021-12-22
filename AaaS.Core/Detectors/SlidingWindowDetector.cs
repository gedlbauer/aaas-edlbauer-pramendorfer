using AaaS.Core.Repositories;
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

        protected DateTime FromDate => DateTime.UtcNow.Subtract(TimeWindow);

        protected SlidingWindowDetector(IMetricRepository metricRepository) : base(metricRepository) { }

        protected abstract Task<double> CalculateCheckValue();

        protected async override Task Detect()
        {
            var value = await CalculateCheckValue();
            if ((UseGreater && value > Threshold) || (!UseGreater && value < Threshold))
            {
                await Action?.Execute();
            }
        }
    }
}
