using AaaS.Common;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public class MSSQLDetectorDao<T> : AdoDetectorDao<T> where T : AaaSAction
    {
        public MSSQLDetectorDao(IConnectionFactory connectionFactory) : base(connectionFactory, new MSSQLClientDao(connectionFactory), new MSSQLActionDao<T>(connectionFactory), new MSSQLObjectPropertyDao(connectionFactory))
        {
        }

        protected override string LastInsertedIdQuery => "SELECT CAST(scope_identity() AS int)";
    }
}
