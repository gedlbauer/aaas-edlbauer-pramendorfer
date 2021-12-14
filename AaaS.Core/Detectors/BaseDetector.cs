using AaaS.Core.Actions;
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
    public abstract class BaseDetector : Detector<BaseAction>, IDetector
    {

        public MetricRepository MetricRepository { set; protected get; }

        public BaseDetector(MetricRepository metricRepository)
        {
            MetricRepository = metricRepository;
        }

        protected abstract Task Detect();

        public async Task Start()
        {
            isRunning = true;
            new Task(async () =>
            {
                while (isRunning)
                {
                    await Detect();
                    Console.WriteLine(TelemetryName +  " detected");
                    await Task.Delay(CheckInterval);
                }
            }).Start();
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
