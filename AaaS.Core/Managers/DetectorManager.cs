using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.Exceptions;
using AaaS.Core.Extensions;
using AaaS.Core.Repositories;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Managers
{
    public class DetectorManager : IDetectorManager
    {
        private readonly List<BaseDetector> _detectors = new();
        private readonly IDetectorDao<BaseDetector, BaseAction> _detectorDao;
        private readonly IClientDao _clientDao;
        private readonly IMetricRepository _metricRepository;
        private readonly IActionManager _actionManager;

        public DetectorManager(IDetectorDao<BaseDetector, BaseAction> detectorDao, IActionManager actionManager, IClientDao clientDao, IMetricRepository metricRepository)
        {
            _detectorDao = detectorDao;
            _actionManager = actionManager;
            _clientDao = clientDao;
            _metricRepository = metricRepository;
            LoadDetectorsFromDb();
            StartAll().Wait();
        }

        private void LoadDetectorsFromDb()
        {
            _detectors.AddRange(_detectorDao.FindAllAsync().ToEnumerable());
            _detectors.ForEach(async detector =>
            {
                detector.MetricRepository = _metricRepository;
                await detector.ResolveNavigationProperties(detector.Action.Id, detector.Client.Id, _actionManager, _clientDao);
            });
        }

        public IEnumerable<BaseDetector> GetAllFromClient(int clientId)
        {
            return _detectors.Where(x => x.Client.Id == clientId);
        }

        public IEnumerable<BaseDetector> GetAll()
        {
            return _detectors;
        }

        public BaseDetector FindDetectorById(int clientId, int id)
        {
            return _detectors.SingleOrDefault(x => x.Client.Id == clientId && x.Id == id);
        }

        public BaseDetector FindDetectorById(int id)
        {
            return _detectors.SingleOrDefault(x => x.Id == id);
        }

        public async Task AddAndStartDetectorAsync(BaseDetector detector)
        {
            if (detector.Id != default)
            {
                throw new ArgumentException("Cannot insert Detector with given id!");
            }
            if (detector.Client is null || await _clientDao.FindByIdAsync(detector.Client.Id) is null)
            {
                throw new ArgumentException("Invalid Client");
            }
            if (detector.Action is null)
            {
                throw new KeyNotFoundException("This action does not exist");
            }
            if (detector.Action.Id == default)
            {
                await _actionManager.AddActionAsync(detector.Action);
            }

            var action = _actionManager.FindActionById(detector.Action.Id);
            if (action is null)
            {
                throw new KeyNotFoundException("This action does not exist");
            }
            if (action.Client.Id != detector.Client.Id)
            {
                throw new AaaSAuthorizationException("This action belongs to a different user!");
            }
            detector.Action = action;


            await _detectorDao.InsertAsync(detector);
            await detector.Start();
            _detectors.Add(detector);
        }

        public void StopAll()
        {
            foreach (var runningDetector in _detectors)
            {
                runningDetector.Stop();
            }
        }

        public async Task StartAll()
        {
            foreach (var runningDetector in _detectors)
            {
                await runningDetector.Start();
            }
        }

        public async Task UpdateDetectorAsync(BaseDetector newDetector)
        {
            var listDetector = _detectors.SingleOrDefault(x => x.Id == newDetector.Id);
            if(newDetector.Client is null)
            {
                throw new ArgumentException("Client must be set!");
            }
            if (listDetector is null)
            {
                throw new ArgumentException("Detector to update must already exit");
            }
            else if (listDetector.GetType() != newDetector.GetType())
            {
                throw new ArgumentException($"Type <{listDetector.GetType()}> does not match Type <{newDetector.GetType()}>!");
            }

            if (newDetector.Action is not null)
            {
                if (newDetector.Action.Client.Id != newDetector.Client.Id)
                {
                    throw new AaaSAuthorizationException("This action belongs to a different user!");
                }
                if (newDetector.Action.Id == default)
                {
                    await _actionManager.AddActionAsync(newDetector.Action);
                }
                else
                {
                    await _actionManager.UpdateActionAsync(newDetector.Action);
                }
            }
            else
            {
                throw new AaaSAuthorizationException("Could not find Action with given id from given user");
            }
            if (await _detectorDao.UpdateAsync(newDetector))
            {
                var detectorType = newDetector.GetType();
                foreach (var property in detectorType.GetProperties().Where(x => x.CanWrite))
                {
                    property.SetValue(listDetector, property.GetValue(newDetector));
                }
            }
        }

        public async Task DeleteDetectorAsync(BaseDetector detectorToDelete)
        {
            if (detectorToDelete is not null)
            {
                detectorToDelete.Stop();
                await _detectorDao.DeleteAsync(detectorToDelete);
                _detectors.RemoveAll(x => x.Id == detectorToDelete.Id);
            }
        }
    }
}
