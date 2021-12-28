using AaaS.Core.Actions;
using AaaS.Core.Repositories;
using AaaS.Dal.Ado.Attributes;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    public abstract class BaseDetector : Detector<BaseAction>, IDetector
    {
        [Volatile]
        public IMetricRepository MetricRepository { set; protected get; }

        public BaseDetector(IMetricRepository metricRepository)
        {
            MetricRepository = metricRepository;
        }

        protected abstract Task Detect();

        public async Task Start()
        {
            IsRunning = true;
            new Task(async () =>
            {
                while (IsRunning)
                {
                    await Detect();
                    await Task.Delay(CheckInterval);
                }
            }).Start();
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
