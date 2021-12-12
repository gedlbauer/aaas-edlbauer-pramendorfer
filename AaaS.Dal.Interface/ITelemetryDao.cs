using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Interface
{
    public interface ITelemetryDao<T> : IBaseDao<T> where T : Telemetry
    {
        Task<T> FindByIdAndKeyAsync(int id, string apiKey);
        IAsyncEnumerable<T> FindByCreatorAsync(string apiKey, Guid creatorId);
        IAsyncEnumerable<T> FindAllByKeyAsync(string apiKey);
        IAsyncEnumerable<T> FindAllByNameAsync(string apiKey, string name);
    }
}
