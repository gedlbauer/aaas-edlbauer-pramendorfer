using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public interface ITelemetryRepository<T> where T : Telemetry
    {
        IAsyncEnumerable<T> FindByCreatorAsync(int clientId, Guid creatorId);
        IAsyncEnumerable<T> FindAllAsync(int clientId);
        IAsyncEnumerable<T> FindByAllByNameAsync(int clientId, string name);
        IAsyncEnumerable<T> FindSinceByClientAsync(DateTime from, int clientId);
        Task<T> FindByIdAsync(int clientId, int id);
        Task InsertAsync(T telemetry);
    }
}
