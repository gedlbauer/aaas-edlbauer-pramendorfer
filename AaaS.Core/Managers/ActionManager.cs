using AaaS.Core.Actions;
using AaaS.Dal.Interface;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Managers
{
    public class ActionManager : IActionManager
    {
        private readonly IActionDao<BaseAction> _actionDao;
        private readonly List<BaseAction> _actions = new();
        private readonly ISendGridClient _sendGridClient;

        public ActionManager(IActionDao<BaseAction> actionDao, ISendGridClient sendGridClient)
        {
            _actionDao = actionDao;
            _sendGridClient = sendGridClient;
            LoadActionsFromDb();
        }

        private void LoadActionsFromDb()
        {
            var actions = _actionDao.FindAllAsync().ToListAsync().Result;
            foreach(var x in actions)
            {
                if (x.GetType() == typeof(MailAction))
                {
                    ((MailAction)x).SetSendGridClient(_sendGridClient);
                }
            }
            _actions.AddRange(actions);
        }

        public BaseAction FindActionById(int id)
        {
            return _actions.SingleOrDefault(x => x.Id == id);
        }

        public BaseAction FindActionById(int clientId, int id)
        {
            return _actions.SingleOrDefault(x => x.Id == id && x.Client.Id == clientId);
        }

        public IEnumerable<BaseAction> GetAll()
        {
            return _actions;
        }

        public IEnumerable<BaseAction> GetAllFromClient(int clientId)
        {
            return _actions.Where(x => x.Client.Id == clientId);
        }

        public async Task AddActionAsync(BaseAction actionToAdd)
        {
            if(actionToAdd.Client is null || actionToAdd.Client.Id == default)
            {
                throw new ArgumentException("Client Id must be set!");
            }
            if(actionToAdd.GetType() == typeof(MailAction))
            {
                ((MailAction)actionToAdd).SetSendGridClient(_sendGridClient);
            }
            _actions.Add(actionToAdd);
            await _actionDao.InsertAsync(actionToAdd);
        }

        public async Task UpdateActionAsync(BaseAction action)
        {
            if (action.Client is null || action.Client.Id == default)
            {
                throw new ArgumentException("Client Id must be set!");
            }
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
                foreach (var property in actionType.GetProperties().Where(prop => prop.CanWrite))
                {
                    property.SetValue(listAction, property.GetValue(action));
                }
            }
        }

        public async Task DeleteActionAsync(BaseAction action)
        {
            if (action is not null)
            {
                await _actionDao.DeleteAsync(action);
                _actions.RemoveAll(x => x.Id == action.Id);
            }
        }
    }
}
