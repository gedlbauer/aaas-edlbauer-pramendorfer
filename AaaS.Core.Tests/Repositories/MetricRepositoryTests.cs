using AaaS.Core.Repositories;
using AaaS.Core.Tests.Infrastructure;
using AaaS.Dal.Interface;
using AaaS.Domain;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Repositories
{
    public class MetricRepositoryTests
    {
        private static Client SampleClient = new()
        {
            Id = 1,
            ApiKey = "api-key",
            Name = "test-user"
        };

        private static Metric SampleMetric = new Metric
        {
            Id = 1,
            Client = SampleClient,
            CreatorId = Guid.Parse("2f729e58-3fe4-4dab-964f-912fc0d845fd"),
            Name = "test-metric",
            Timestamp = DateTime.Now,
            Value = 1
        };

        private static IAsyncEnumerable<Metric> Metrics = new List<Metric>
        {
            SampleMetric,
            new Metric
            {
                Id = 2,
                Client = SampleClient,
                CreatorId = Guid.Parse("2f729e58-3fe4-4dab-964f-912fc0d845fd"),
                Name = "test-metric",
                Timestamp = DateTime.Now,
                Value = 10
            }
        }.ToAsyncEnumerable();

        [Fact]
        public async Task TestInsertMeasurement()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            await metricRepo.InsertAsync(SampleMetric);
            await metricRepo.InsertMeasurementAsync(SampleMetric);

            metricDaoMock.Verify(x => x.InsertAsync(It.IsAny<Metric>()), Times.Exactly(2));
        }

        [Fact]
        public async Task TestInsertCounter()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            metricDaoMock.Setup(x => x.FindMostRecentByNameAndClientAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Metric { Value = 10 }));
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            await metricRepo.InsertCounterAsync(SampleMetric);

            metricDaoMock.Verify(x => x.InsertAsync(It.Is<Metric>(x => x.Value == 11)), Times.Exactly(1));
        }

        [Fact]
        public async Task TestInsertFirstCounter()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            await metricRepo.InsertCounterAsync(SampleMetric);

            metricDaoMock.Verify(x => x.InsertAsync(It.Is<Metric>(x => x.Value == 1)), Times.Exactly(1));
        }

        [Fact]
        public async Task TestFindAll()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            metricDaoMock.Setup(x => x.FindAllByClientAsync(It.IsAny<int>()))
                .Returns(Metrics);
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            (await metricRepo.FindAllAsync(1).ToListAsync())
                .Should()
                .BeEquivalentTo(await Metrics.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindByName()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            metricDaoMock.Setup(x => x.FindAllByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Metrics);
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            (await metricRepo.FindByAllByNameAsync(1, "test-name").ToListAsync())
                .Should()
                .BeEquivalentTo(await Metrics.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindByCreator()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            metricDaoMock.Setup(x => x.FindByCreatorAsync(It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns(Metrics);
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            (await metricRepo.FindByCreatorAsync(1, Guid.NewGuid()).ToListAsync())
                .Should()
                .BeEquivalentTo(await Metrics.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindById()
        {
            var metricDaoMock = new Mock<IMetricDao>();
            metricDaoMock.Setup(x => x.FindByIdAndClientAsync(It.IsAny<int>(), 1))
                .Returns(Task.FromResult(SampleMetric));
            var metricRepo = new MetricRepository(metricDaoMock.Object);

            (await metricRepo.FindByIdAsync(1, 1))
                .Should()
                .BeEquivalentTo(SampleMetric, FluentAssertionsExtensions.ApproximateDateTime);
        }
    }
}
