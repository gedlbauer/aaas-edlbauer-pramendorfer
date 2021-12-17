using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ILogDao _logDao;

        public LogRepository(ILogDao logDao)
        {
            _logDao = logDao;
        }

        public IAsyncEnumerable<Log> FindAllAsync(int clientId)
            => _logDao.FindAllByClientAsync(clientId);

        public IAsyncEnumerable<LogType> FindAllLogTypesAsync()
            => _logDao.FindAllLogTypesAsync();

        public IAsyncEnumerable<Log> FindByAllByNameAsync(int clientId, string name)
            => _logDao.FindAllByNameAsync(clientId, name);

        public IAsyncEnumerable<Log> FindByCreatorAsync(int clientId, Guid creatorId)
            => _logDao.FindByCreatorAsync(clientId, creatorId);

        public Task<Log> FindByIdAsync(int clientId, int id)
            => _logDao.FindByIdAndClientAsync(id, clientId);

        public IAsyncEnumerable<Log> FindSinceByClientAndTelemetryNameAsync(DateTime from, int clientId, string telemetryName)
            => _logDao.FindSinceByClientAndTelemetryNameAsync(from, clientId, telemetryName);

        public async Task InsertAsync(Log telemetry)
            => await _logDao.InsertAsync(telemetry);

    }
}
