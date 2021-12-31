using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Interface
{
    public interface IMetricDao : ITelemetryDao<Metric>
    {
        Task<Metric> FindMostRecentByNameAndClientAsync(int clientId, string name);
        IAsyncEnumerable<string> FindNamesByClientAsync(int clientId);
    }
}
