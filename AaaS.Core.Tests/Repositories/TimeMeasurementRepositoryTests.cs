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
    public class TimeMeasurementRepositoryTests
    {
        private static Client SampleClient = new()
        {
            Id = 1,
            ApiKey = "api-key",
            Name = "test-user"
        };

        private static TimeMeasurement SampleMeasurement = new()
        {
            Id = 1,
            Client = SampleClient,
            CreatorId = Guid.NewGuid(),
            Name = "timing",
            StartTime = DateTime.Now.AddSeconds(-10),
            EndTime = DateTime.Now,
            Timestamp = DateTime.Now
        };

        private static IAsyncEnumerable<TimeMeasurement> TimeMeasurements = new List<TimeMeasurement>() {
            SampleMeasurement,
            new TimeMeasurement
            {
                Id = 2,
                Client = SampleClient,
                CreatorId = Guid.NewGuid(),
                Name = "timing",
                StartTime = DateTime.Now.AddSeconds(-20),
                EndTime = DateTime.Now,
                Timestamp = DateTime.Now
            }
        }.ToAsyncEnumerable();

        [Fact]
        public async Task TestInsertTimeMeasurement()
        {
            var timeDao = new Mock<ITimeMeasurementDao>();
            var timeRepo = new TimeMeasurementRepository(timeDao.Object);

            await timeRepo.InsertAsync(SampleMeasurement);

            timeDao.Verify(x => x.InsertAsync(It.IsAny<TimeMeasurement>()), Times.Once);
        }

        [Fact]
        public async Task TestFindAll()
        {
            var timeDao = new Mock<ITimeMeasurementDao>();
            timeDao.Setup(x => x.FindAllByClientAsync(It.IsAny<int>()))
                .Returns(TimeMeasurements);
            var timeRepo = new TimeMeasurementRepository(timeDao.Object);

            (await timeRepo.FindAllAsync(1).ToListAsync())
                .Should()
                .BeEquivalentTo(await TimeMeasurements.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindByName()
        {
            var timeDao = new Mock<ITimeMeasurementDao>();
            timeDao.Setup(x => x.FindAllByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(TimeMeasurements);
            var timeRepo = new TimeMeasurementRepository(timeDao.Object);


            (await timeRepo.FindByAllByNameAsync(1, "test-name").ToListAsync())
                .Should()
                .BeEquivalentTo(await TimeMeasurements.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindByCreator()
        {
            var timeDao = new Mock<ITimeMeasurementDao>();
            timeDao.Setup(x => x.FindByCreatorAsync(It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns(TimeMeasurements);
            var timeRepo = new TimeMeasurementRepository(timeDao.Object);

            (await timeRepo.FindByCreatorAsync(1, Guid.NewGuid()).ToListAsync())
                .Should()
                .BeEquivalentTo(await TimeMeasurements.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindById()
        {
            var timeDao = new Mock<ITimeMeasurementDao>();
            timeDao.Setup(x => x.FindByIdAndClientAsync(1, 1))
                .Returns(Task.FromResult(SampleMeasurement));
            var timeRepo = new TimeMeasurementRepository(timeDao.Object);

            (await timeRepo.FindByIdAsync(1, 1))
                .Should()
                .BeEquivalentTo(SampleMeasurement, FluentAssertionsExtensions.ApproximateDateTime);
            (await timeRepo.FindByIdAsync(10, 3)).Should().BeNull();
        }
    }
}
