using AaaS.Core.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Managers
{
    public interface IActionManager
    {
        BaseAction FindActionById(int id);
        BaseAction FindActionById(int clientId, int id);
        public IEnumerable<BaseAction> GetAll();
        public IEnumerable<BaseAction> GetAllFromClient(int clientId);
        public Task AddActionAsync(BaseAction actionToAdd);
        public Task UpdateActionAsync(BaseAction action);
        public Task DeleteActionAsync(BaseAction action);

    }
}
