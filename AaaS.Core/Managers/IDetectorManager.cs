using AaaS.Core.Detectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Managers
{
    public interface IDetectorManager
    {
        public IEnumerable<BaseDetector> GetAllFromClient(int clientId);
        public IEnumerable<BaseDetector> GetAll();
        public BaseDetector FindDetectorById(int clientId, int id);
        public BaseDetector FindDetectorById(int id);
        public Task AddAndStartDetectorAsync(BaseDetector detector);
        public void StopAll();
        public Task StartAll();
        public Task UpdateDetectorAsync(BaseDetector newDetector);
        public Task DeleteDetectorAsync(BaseDetector detectorToDelete);
    }
}
