using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public class LogRepository : ITelemetryRepository<Log>
    {
        private readonly ILogDao _logDao;

        public LogRepository(ILogDao logDao)
        {
            _logDao = logDao;
        }

        public IAsyncEnumerable<Log> FindAllAsync(string apiKey)
            => _logDao.FindAllByKeyAsync(apiKey);

        public IAsyncEnumerable<Log> FindByAllByNameAsync(string apiKey, string name)
            => _logDao.FindAllByNameAsync(apiKey, name);

        public IAsyncEnumerable<Log> FindByCreatorAsync(string apiKey, Guid creatorId)
            => _logDao.FindByCreatorAsync(apiKey, creatorId);

        public Task<Log> FindByIdAsync(string apiKey, int id)
            => _logDao.FindByIdAndKeyAsync(id, apiKey);

        public async Task InsertAsync(Log telemetry)
        {
            await _logDao.InsertAsync(telemetry);
        }
    }
}
