using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Interface
{
    public interface ITimeMeasurementDao : ITelemetryDao<TimeMeasurement>
    {
    }
}
