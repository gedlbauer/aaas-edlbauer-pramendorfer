using AaaS.Core.Detectors;
using AaaS.Core.Managers;
using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Extensions
{
    public static class DetectorExtensions
    {
        public static async Task ResolveNavigationProperties(this BaseDetector detector, int actionId, int clientId, ActionManager actionManager, IClientDao clientDao)
        {
            detector.Action = actionManager.FindActionById(actionId);
            detector.Client = await clientDao.FindByIdAsync(clientId);
        }
    }
}
