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
        public IAsyncEnumerable<TimeMeasurement> FindAllAsync(string apiKey)
            => _timeMeasurementDao.FindAllByKeyAsync(apiKey).Take(1000);

        public IAsyncEnumerable<TimeMeasurement> FindByAllByNameAsync(string apiKey, string name)
            => _timeMeasurementDao.FindAllByNameAsync(apiKey, name);

        public IAsyncEnumerable<TimeMeasurement> FindByCreatorAsync(string apiKey, Guid creatorId)
            => _timeMeasurementDao.FindByCreatorAsync(apiKey, creatorId);

        public Task<TimeMeasurement> FindByIdAsync(string apiKey, int id)
            => _timeMeasurementDao.FindByIdAndKeyAsync(id, apiKey);

        public Task InsertAsync(TimeMeasurement telemetry)
            => _timeMeasurementDao.InsertAsync(telemetry);
    }
}
