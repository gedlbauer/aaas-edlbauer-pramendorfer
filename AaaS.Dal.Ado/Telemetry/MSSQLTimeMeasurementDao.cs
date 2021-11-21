using AaaS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado.Telemetry
{
    public class MSSQLTimeMeasurementDao : AdoTimeMesurementDao
    {
        protected override string LastInsertedIdQuery => "SELECT CAST(scope_identity() AS int)";

        public MSSQLTimeMeasurementDao(IConnectionFactory factory) : base(factory, new MSSQLClientDao(factory)) { }
    }
}
