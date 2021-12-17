using AaaS.ClientSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AaaS.DemoClient
{
    public class AaaSDemo
    {
        private readonly Guid _creatorId;
        private readonly int _delay;
        private readonly AaaSService _aaasService;
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _ramCounter;
        private readonly Random _random = new();
        private CancellationTokenSource TokenSource { get; set; }

        public IEnumerable<LogType> LogTypes { get; private set; }

        /**
         * Only works on windows os
         */
        public AaaSDemo(AaaSService aaaSService, int eventsPerMinute)
        {
            _delay = 60000 / eventsPerMinute;
            _aaasService = aaaSService;
            _creatorId = Guid.NewGuid();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public void Start()
        {
            TokenSource?.Cancel();
            TokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(async () =>
            {
                LogTypes = await _aaasService.GetLogTypes();
                while (true)
                {
                    await MetricCpu();
                    await MetricRam();
                    await Log();
                    await TimeMeasurement();
                    await Task.Delay(_delay);
                }
            }, TokenSource.Token);
        }

        public async Task Stop()
        {
            await _aaasService.Stop();
            TokenSource.Cancel();
        }

        private async Task Log()
        {
            await _aaasService.InsertLog(new LogInsertDto
            {
                CreatorId = _creatorId,
                LogTypeId = LogTypes.ElementAt(_random.Next(0, LogTypes.Count())).Id,
                Message = $"Mouse at {Console.CursorLeft} / {Console.CursorTop}",
                Name = "Cursor Position",
                Timestamp = DateTime.Now
            });
        }

        private async Task MetricCpu()
        {
            await _aaasService.InsertMeasurement(new MeasurementInsertDto
            {
                CreatorId = _creatorId,
                Name = "CPU Usage",
                Timestamp = DateTime.Now,
                Value = _cpuCounter.NextValue()
            });
        }

        private async Task MetricRam()
        {
            await _aaasService.InsertMeasurement(new MeasurementInsertDto
            {
                CreatorId = _creatorId,
                Name = "Free Ram",
                Timestamp = DateTime.Now,
                Value = _ramCounter.NextValue()
            });
        }

        private async Task TimeMeasurement()
        {
            _aaasService.StartTiming($"Random Time Measurement", _creatorId);
            await Task.Delay(_random.Next(0, _delay));
            await _aaasService.StopAndInsertTiming($"Random Time Measurement");
        }
    }
}
