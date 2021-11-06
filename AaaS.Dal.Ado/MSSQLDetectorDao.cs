using AaaS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public class MSSQLDetectorDao : AdoDetectorDao
    {
        protected override string LastInsertedIdQuery => "SELECT CAST(scope_identity() AS int)";
    }
}
