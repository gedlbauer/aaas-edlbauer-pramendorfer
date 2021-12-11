using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Managers
{
    public class DetectorManager
    {
        private readonly IDetectorDao<BaseDetector, BaseAction> _detectorDao;
        private readonly List<BaseDetector> _runningDetectors = new();
        private readonly ActionManager _actionManager;

        public DetectorManager(IDetectorDao<BaseDetector, BaseAction> detectorDao, ActionManager actionManager)
        {
            _detectorDao = detectorDao;
            _actionManager = actionManager;
            LoadDetectorsFromDb();
        }

        private void LoadDetectorsFromDb()
        {
            _runningDetectors.AddRange(_detectorDao.FindAllAsync().ToEnumerable());
            _runningDetectors.ForEach(x => x.Action = _actionManager.FindActionById(x.Action.Id));
        }

        public async Task AddAndStartDetectorAsync(BaseDetector detector)
        {
            if (detector.Id != default)
            {
                throw new ArgumentException("Cannot insert Detector with given id!");
            }

            if (detector.Action.Id == default)
            {
                await _actionManager.AddActionAsync(detector.Action);
            } else
            {
                detector.Action = _actionManager.FindActionById(detector.Action.Id);
            }

            await _detectorDao.InsertAsync(detector);
            await detector.Start();
            _runningDetectors.Add(detector);
        }

        public void StopAll()
        {
            foreach (var runningDetector in _runningDetectors)
            {
                runningDetector.Stop();
            }
        }

        public async Task StartAll()
        {
            foreach (var runningDetector in _runningDetectors)
            {
                await runningDetector.Start();
            }
        }

        public async Task UpdateDetectorAsync(BaseDetector newDetector)
        {
            var listDetector = _runningDetectors.SingleOrDefault(x => x.Id == newDetector.Id);
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
                if (newDetector.Action.Id == default)
                {
                    await _actionManager.AddActionAsync(newDetector.Action);
                }
                else
                {
                    await _actionManager.UpdateActionAsync(newDetector.Action);
                }
            }
            if (await _detectorDao.UpdateAsync(newDetector))
            {
                var detectorType = newDetector.GetType();
                foreach (var property in detectorType.GetProperties())
                {
                    property.SetValue(listDetector, property.GetValue(newDetector));
                }
            }
        }

        public async Task DeleteDetectorAsync(BaseDetector detectorToDelete)
        {
            detectorToDelete.Stop();
            await _detectorDao.DeleteAsync(detectorToDelete);
            _runningDetectors.RemoveAll(x => x.Id == detectorToDelete.Id);
        }
    }
}
