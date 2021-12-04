using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Detectors
{
    interface IDetector
    {
        Task Start();
        void Stop();
    }
}
