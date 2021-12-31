using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public interface IMetricRepository : ITelemetryRepository<Metric>
    {
        Task InsertCounterAsync(Metric metric);
        Task InsertMeasurementAsync(Metric metric);
        IAsyncEnumerable<string> FindAllMetricNamesFromClientAsync(int clientId);
    }
}
