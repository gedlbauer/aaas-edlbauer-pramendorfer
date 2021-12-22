using AaaS.Core.Detectors;
using AaaS.Core.Repositories;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Tests.TestObjects
{
    public class SimpleDetector : BaseDetector
    {
        public int SimpleValue { get; set; }
        public SimpleDetector(MetricRepository metricRepository) : base(metricRepository) { }

        public SimpleDetector() : base(null) { }

        public string Name { get; set; }
        public override string ToString()
        {
            return $"Hello I am a SimpleDetector! (${base.ToString()})";
        }

        protected override Task Detect()
        {
            return Task.FromResult(new object());
        }
    }
}
