using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public class MetricRepository : ITelemetryRepository<Metric>
    {
        private readonly IMetricDao _metricDao;

        public MetricRepository(IMetricDao metricDao)
        {
            _metricDao = metricDao;
        }
        public IAsyncEnumerable<Metric> FindAllAsync(int clientId)
            => _metricDao.FindAllByClientAsync(clientId);

        public IAsyncEnumerable<Metric> FindSinceByClientAsync(DateTime fromDate, int clientId)
            => _metricDao.FindSinceByClientAsync(fromDate, clientId);

        public IAsyncEnumerable<Metric> FindByAllByNameAsync(int clientId, string name)
            => _metricDao.FindAllByNameAsync(clientId, name);

        public IAsyncEnumerable<Metric> FindByCreatorAsync(int clientId, Guid creatorId)
            => _metricDao.FindByCreatorAsync(clientId, creatorId);

        public Task<Metric> FindByIdAsync(int clientId, int id)
            => _metricDao.FindByIdAndClientAsync(id, clientId);

        public Task InsertAsync(Metric telemetry)
            => _metricDao.InsertAsync(telemetry);
    }
}
