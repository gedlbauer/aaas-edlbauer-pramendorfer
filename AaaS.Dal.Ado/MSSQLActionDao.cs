using AaaS.Common;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public class MSSQLActionDao<T> : AdoActionDao<T> where T : AaaSAction
    {
        public MSSQLActionDao(IConnectionFactory factory) : base(factory, new MSSQLObjectPropertyDao(factory), new MSSQLClientDao(factory))
        {
        }

        protected override string LastInsertedIdQuery => "SELECT CAST(scope_identity() AS int)";
    }
}
