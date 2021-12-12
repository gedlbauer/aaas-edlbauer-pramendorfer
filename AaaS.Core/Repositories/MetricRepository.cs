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
        public IAsyncEnumerable<Metric> FindAllAsync(string apiKey)
            => _metricDao.FindAllByKeyAsync(apiKey).Take(1000);

        public IAsyncEnumerable<Metric> FindByAllByNameAsync(string apiKey, string name)
            => _metricDao.FindAllByNameAsync(apiKey, name);

        public IAsyncEnumerable<Metric> FindByCreatorAsync(string apiKey, Guid creatorId)
            => _metricDao.FindByCreatorAsync(apiKey, creatorId);

        public Task<Metric> FindByIdAsync(string apiKey, int id)
            => _metricDao.FindByIdAndKeyAsync(id, apiKey);

        public Task InsertAsync(Metric telemetry)
            => _metricDao.InsertAsync(telemetry);
    }
}
