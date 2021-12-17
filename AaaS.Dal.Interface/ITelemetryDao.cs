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
        Task<T> FindByIdAndClientAsync(int id, int clientId);
        IAsyncEnumerable<T> FindByCreatorAsync(int clientId, Guid creatorId);
        IAsyncEnumerable<T> FindAllByClientAsync(int clientId);
        IAsyncEnumerable<T> FindAllByNameAsync(int clientId, string name);
        IAsyncEnumerable<T> FindSinceByClientAndTelemetryNameAsync(DateTime from, int clientId, string telemetryName);
    }
}
