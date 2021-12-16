using AaaS.Core.Actions;
using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Extensions
{
    public static class ActionExtensions
    {
        public static async Task ResolveNavigationProperties(this BaseAction action, int clientId, IClientDao clientDao)
        {
            action.Client = await clientDao.FindByIdAsync(clientId);
        }
    }
}
