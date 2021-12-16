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
        private static int cnt = 0;
        public BaseAction()
        {
            Console.WriteLine($"Action Created! (#{cnt++})");
        }
        public abstract Task Execute();
    }
}
