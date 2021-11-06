using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public interface IAction
    {
        public int Id { get; set; }

        void Execute();
    }
}
