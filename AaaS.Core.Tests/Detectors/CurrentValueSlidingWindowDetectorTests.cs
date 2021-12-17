﻿using AaaS.Core.Detectors;
using AaaS.Core.Repositories;
using AaaS.Domain;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Detectors
{
    public class CurrentValueSlidingWindowDetectorTests
    {
        private class CurrentValueSlidingWindowDetectorWrapper : CurrentValueSlidingWindowDetector
        {
            public CurrentValueSlidingWindowDetectorWrapper(IMetricRepository metricRepository) : base(metricRepository)
            {
            }

            public Task<double> ExecuteCheck()
            {
                return base.CalculateCheckValue();
            }
        }

        private static readonly Client SampleClient = new()
        {
            Name = "Sample",
            ApiKey = "SampleKey",
            Id = 1
        };
        private static IAsyncEnumerable<Metric> SampleMetrics => new List<Metric>
        {
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-10), Name="Sample Metric", Client=SampleClient, Id=1, CreatorId=Guid.Empty, Value=1},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-9), Name="Sample Metric", Client=SampleClient, Id=2, CreatorId=Guid.Empty, Value=2},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-8), Name="Sample Metric", Client=SampleClient, Id=3, CreatorId=Guid.Empty, Value=3},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-7), Name="Sample Metric", Client=SampleClient, Id=4, CreatorId=Guid.Empty, Value=4},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-6), Name="Sample Metric", Client=SampleClient, Id=5, CreatorId=Guid.Empty, Value=5},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-5), Name="Sample Metric", Client=SampleClient, Id=6, CreatorId=Guid.Empty, Value=6},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-4), Name="Sample Metric", Client=SampleClient, Id=7, CreatorId=Guid.Empty, Value=7},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-3), Name="Sample Metric", Client=SampleClient, Id=8, CreatorId=Guid.Empty, Value=8},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-2), Name="Sample Metric", Client=SampleClient, Id=9, CreatorId=Guid.Empty, Value=9},
            new Metric{Timestamp=DateTime.UtcNow.AddMinutes(-1), Name="Sample Metric", Client=SampleClient, Id=10, CreatorId=Guid.Empty, Value=10}
        }.ToAsyncEnumerable();

        [Fact]
        public async Task ReturnsCorrectValueIfValueExistsWithinTimeSpan()
        {
            var repoMock = new Mock<IMetricRepository>();
            repoMock.Setup(repo => repo.FindSinceByClientAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns<DateTime, int>((fromDate, clientId) => SampleMetrics.Where(x => x.Timestamp >= fromDate));
            var currValDetector = new CurrentValueSlidingWindowDetectorWrapper(repoMock.Object)
            {
                CheckInterval = TimeSpan.FromMinutes(10),
                Client = new Client { Id = 1 },
                TelemetryName = "Sample Metric",
                TimeWindow = TimeSpan.FromMinutes(10)
            };
            var result = await currValDetector.ExecuteCheck();

            Assert.Equal(10, result);
        }

        [Fact]
        public async Task ReturnsCorrectValueIfNoValueExistsWithinTimeSpan()
        {
            var repoMock = new Mock<IMetricRepository>();
            repoMock.Setup(repo => repo.FindSinceByClientAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns<DateTime, int>((fromDate, clientId) => SampleMetrics.Where(x => x.Timestamp >= fromDate));
            var currValDetector = new CurrentValueSlidingWindowDetectorWrapper(repoMock.Object)
            {
                CheckInterval = TimeSpan.FromSeconds(10),
                Client = new Client { Id = 1 },
                TelemetryName = "Sample Metric",
                TimeWindow = TimeSpan.FromSeconds(0)
            };
            var result = await currValDetector.ExecuteCheck();

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task IgnoresWrongTelemetries()
        {
            var repoMock = new Mock<IMetricRepository>();
            repoMock.Setup(repo => repo.FindSinceByClientAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns<DateTime, int>((fromDate, clientId) => SampleMetrics.Where(x => x.Timestamp >= fromDate));
            var currValDetector = new CurrentValueSlidingWindowDetectorWrapper(repoMock.Object)
            {
                CheckInterval = TimeSpan.FromMinutes(10),
                Client = new Client { Id = 1 },
                TelemetryName = "Wrong Metric",
                TimeWindow = TimeSpan.FromMinutes(10)
            };
            var result = await currValDetector.ExecuteCheck();

            Assert.Equal(0, result);
        }
    }
}
