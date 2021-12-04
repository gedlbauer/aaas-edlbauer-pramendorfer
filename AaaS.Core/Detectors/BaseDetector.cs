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
        protected abstract void Detect();

        public async Task Start()
        {
            isRunning = true;
            while (isRunning)
            {
                Detect();
                await Task.Delay(CheckInterval);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
