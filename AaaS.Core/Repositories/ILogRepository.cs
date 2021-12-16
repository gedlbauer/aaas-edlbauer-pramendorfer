using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public interface ILogRepository : ITelemetryRepository<Log>
    {
        IAsyncEnumerable<LogType> FindAllLogTypesAsync();
    }
}
