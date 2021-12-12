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
        IAsyncEnumerable<T> FindByCreatorAsync(string apiKey, Guid creatorId);
        IAsyncEnumerable<T> FindAllAsync(string apiKey);
        IAsyncEnumerable<T> FindByAllByNameAsync(string apiKey, string name);
        Task<T> FindByIdAsync(string apiKey, int id);
        Task InsertAsync(T telemetry);
    }
}
