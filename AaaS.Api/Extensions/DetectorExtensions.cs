using AaaS.Api.Dtos.Detector;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.Managers;
using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaaS.Api.Extensions
{
    public static class DetectorExtensions
    {
        public static async Task ResolveNavigationProperties(this BaseDetector detector, DetectorInsertBaseDto detectorDto, int clientId, ActionManager actionManager, IClientDao clientDao)
        {
            detector.Action = actionManager.FindActionById(detectorDto.ActionId);
            detector.Client = await clientDao.FindByIdAsync(clientId);
        }
    }
}
