using AaaS.Core.Actions;
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
        protected abstract Task Detect();

        public async Task Start()
        {
            isRunning = true;
            while (isRunning)
            {
                await Detect();
                await Task.Delay(CheckInterval);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
