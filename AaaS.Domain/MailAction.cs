using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public class MailAction : IAction
    {
        public int Id { get; set; }
        public string MailAddress { get; set; }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
