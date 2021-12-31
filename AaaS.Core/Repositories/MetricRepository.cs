using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly IMetricDao _metricDao;

        public MetricRepository(IMetricDao metricDao)
        {
            _metricDao = metricDao;
        }
        public IAsyncEnumerable<Metric> FindAllAsync(int clientId)
            => _metricDao.FindAllByClientAsync(clientId);

        public IAsyncEnumerable<Metric> FindSinceByClientAndTelemetryNameAsync(DateTime fromDate, int clientId, string telemetryName)
            => _metricDao.FindSinceByClientAndTelemetryNameAsync(fromDate, clientId, telemetryName);

        public IAsyncEnumerable<Metric> FindByAllByNameAsync(int clientId, string name)
            => _metricDao.FindAllByNameAsync(clientId, name);

        public IAsyncEnumerable<Metric> FindByCreatorAsync(int clientId, Guid creatorId)
            => _metricDao.FindByCreatorAsync(clientId, creatorId);

        public Task<Metric> FindByIdAsync(int clientId, int id)
            => _metricDao.FindByIdAndClientAsync(id, clientId);

        public Task InsertAsync(Metric telemetry)
           => InsertMeasurementAsync(telemetry);

        public async Task InsertMeasurementAsync(Metric telemetry)
            => await _metricDao.InsertAsync(telemetry);

        public async Task InsertCounterAsync(Metric telemetry)
        {
            Metric latestMetric = await _metricDao.FindMostRecentByNameAndClientAsync(telemetry.Client.Id, telemetry.Name);
            telemetry.Value = (latestMetric?.Value ?? 0) + 1;
            await _metricDao.InsertAsync(telemetry);
        }

        public IAsyncEnumerable<string> FindAllMetricNamesFromClientAsync(int clientId)
        {
            return _metricDao.FindNamesByClientAsync(clientId);
        }
    }
}
