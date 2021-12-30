using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Domain
{
    public abstract class AaaSAction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Client Client { get; set; }
        public string TypeName => GetType().Name;
    }
}
