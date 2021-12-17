using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.Repositories;
using AaaS.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Detectors
{
    public class MinMaxDetectorTests
    {
        private static readonly Client SampleClient = new()
        {
            Name = "Sample",
            ApiKey = "SampleKey",
            Id = 1
        };

        private static IAsyncEnumerable<Metric> SampleMetrics => new List<Metric>
        {
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-9), Name="Sample Metric", Client=SampleClient, Id=1, CreatorId=Guid.Empty, Value=1},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-8), Name="Sample Metric", Client=SampleClient, Id=2, CreatorId=Guid.Empty, Value=2},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-7), Name="Sample Metric", Client=SampleClient, Id=3, CreatorId=Guid.Empty, Value=3},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-6), Name="Sample Metric", Client=SampleClient, Id=4, CreatorId=Guid.Empty, Value=4},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-5), Name="Sample Metric", Client=SampleClient, Id=5, CreatorId=Guid.Empty, Value=5},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-4), Name="Sample Metric", Client=SampleClient, Id=6, CreatorId=Guid.Empty, Value=6},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-3), Name="Sample Metric", Client=SampleClient, Id=7, CreatorId=Guid.Empty, Value=7},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-2), Name="Sample Metric", Client=SampleClient, Id=8, CreatorId=Guid.Empty, Value=8},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-1), Name="Sample Metric", Client=SampleClient, Id=9, CreatorId=Guid.Empty, Value=9},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-0), Name="Sample Metric", Client=SampleClient, Id=10, CreatorId=Guid.Empty, Value=10}
        }.ToAsyncEnumerable();

        [Fact]
        public async Task TestDetectorExecutesOnAnomaly()
        {
            var metricRepoMock = new Mock<IMetricRepository>();
            metricRepoMock.Setup(repo => repo.FindSinceByClientAndTelemetryNameAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>())).Returns(SampleMetrics);

            var actionMock = new Mock<BaseAction>();

            var minMaxDetector = new MinMaxDetector(metricRepoMock.Object)
            {
                Max = 6,
                Min = 2,
                MaxOccurs = 4,
                CheckInterval = TimeSpan.FromMinutes(60),
                TelemetryName = "Sample Metric",
                TimeWindow = TimeSpan.FromMinutes(15),
                Id = 1,
                Client = SampleClient,
                Action = actionMock.Object
            };
            await minMaxDetector.Start();
            await Task.Delay(100);
            minMaxDetector.Stop();
            actionMock.Verify(action => action.Execute(), Times.Once());
        }

        [Fact]
        public async Task TestDetectorDoesNotExecuteOnNoAnomaly()
        {
            var metricRepoMock = new Mock<IMetricRepository>();
            metricRepoMock.Setup(repo => repo.FindSinceByClientAndTelemetryNameAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>())).Returns(SampleMetrics);

            var actionMock = new Mock<BaseAction>();

            var minMaxDetector = new MinMaxDetector(metricRepoMock.Object)
            {
                Max = 7,
                Min = 2,
                MaxOccurs = 4,
                CheckInterval = TimeSpan.FromMilliseconds(10),
                TelemetryName = "Sample Metric",
                TimeWindow = TimeSpan.FromMinutes(15),
                Id = 1,
                Client = SampleClient,
                Action = actionMock.Object
            };
            await minMaxDetector.Start();
            await Task.Delay(100);
            minMaxDetector.Stop();
            actionMock.Verify(action => action.Execute(), Times.Never());
        }

    }
}
