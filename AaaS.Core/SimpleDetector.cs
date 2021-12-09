using AaaS.Core.Detectors;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core
{
    public class SimpleDetector : BaseDetector
    {
        public SimpleDetector(IMetricDao metricDao = null) : base(metricDao)
        {
        }

        public string Name { get; set; }
        public override string ToString()
        {
            return $"Hello I am a SimpleDetector! (${base.ToString()})";
        }

        protected override Task Detect()
        {
            throw new NotImplementedException();
        }
    }
}
