using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Repositories
{
    public class TimeMeasurementRepository : ITelemetryRepository<TimeMeasurement>
    {
        private readonly ITimeMeasurementDao _timeMeasurementDao;

        public TimeMeasurementRepository(ITimeMeasurementDao timeMeasurementDao)
        {
            _timeMeasurementDao = timeMeasurementDao;
        }
        public IAsyncEnumerable<TimeMeasurement> FindAllAsync(int clientId)
            => _timeMeasurementDao.FindAllByClientAsync(clientId);

        public IAsyncEnumerable<TimeMeasurement> FindByAllByNameAsync(int clientId, string name)
            => _timeMeasurementDao.FindAllByNameAsync(clientId, name);

        public IAsyncEnumerable<TimeMeasurement> FindByCreatorAsync(int clientId, Guid creatorId)
            => _timeMeasurementDao.FindByCreatorAsync(clientId, creatorId);

        public Task<TimeMeasurement> FindByIdAsync(int clientId, int id)
            => _timeMeasurementDao.FindByIdAndClientAsync(id, clientId);

        public IAsyncEnumerable<TimeMeasurement> FindSinceByClientAndTelemetryNameAsync(DateTime from, int clientId, string telemetryName)
            => _timeMeasurementDao.FindSinceByClientAndTelemetryNameAsync(from, clientId, telemetryName);

        public Task InsertAsync(TimeMeasurement telemetry)
            => _timeMeasurementDao.InsertAsync(telemetry);
    }
}
