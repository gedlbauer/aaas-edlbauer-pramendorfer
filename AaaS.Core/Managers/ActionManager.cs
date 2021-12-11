using AaaS.Core.Actions;
using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Managers
{
    public class ActionManager
    {
        private readonly IActionDao<BaseAction> _actionDao;
        private readonly List<BaseAction> _actions = new();

        public ActionManager(IActionDao<BaseAction> actionDao)
        {
            _actionDao = actionDao;
            LoadActionsFromDb();
        }

        private void LoadActionsFromDb()
        {
            _actions.AddRange(_actionDao.FindAllAsync().ToEnumerable());
        }

        public BaseAction FindActionById(int id)
        {
            return _actions.SingleOrDefault(x => x.Id == id);
        }

        public async Task AddActionAsync(BaseAction actionToAdd)
        {
            _actions.Add(actionToAdd);
            await _actionDao.InsertAsync(actionToAdd);
        }

        public async Task UpdateActionAsync(BaseAction action)
        {
            var listAction = _actions.SingleOrDefault(x => x.Id == action.Id);
            if (listAction is null)
            {
                throw new ArgumentException("Action to update must already exit");
            }
            else if (listAction.GetType() != action.GetType())
            {
                throw new ArgumentException($"Type <{listAction.GetType()}> does not match Type <{action.GetType()}>!");
            }
            if (await _actionDao.UpdateAsync(action))
            {
                var actionType = action.GetType();
                foreach (var property in actionType.GetProperties())
                {
                    property.SetValue(listAction, property.GetValue(action));
                }
            }
        }

        public async Task DeleteActionAsync(BaseAction action)
        {
            await _actionDao.DeleteAsync(action);
            _actions.RemoveAll(x => x.Id == action.Id);
        }
    }
}
