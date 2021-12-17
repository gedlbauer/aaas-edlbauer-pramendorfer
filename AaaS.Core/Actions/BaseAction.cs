using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Actions
{
    public abstract class BaseAction : AaaSAction, IAction
    {
        public BaseAction() { }
        public abstract Task Execute();
    }
}
