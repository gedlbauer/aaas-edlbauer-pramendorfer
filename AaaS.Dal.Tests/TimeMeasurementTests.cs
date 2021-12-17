using AaaS.Dal.Ado.Telemetry;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
using AaaS.Dal.Tests.Infrastructure;
using AaaS.Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Dal.Tests
{
    [Collection("SeededDb")]
    public class TimeMeasurementTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly ITimeMeasurementDao timeDao;

        public TimeMeasurementTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            timeDao = new MSSQLTimeMeasurementDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsertMetric()
        {
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            TimeMeasurement measurement = new TimeMeasurement { Client = client, CreatorId = Guid.NewGuid(), Name = "testname", Timestamp = DateTime.Now, StartTime = DateTime.Now.AddDays(-1), EndTime = DateTime.Now };

            await timeDao.InsertAsync(measurement);

            measurement.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            TimeMeasurement measurement = new TimeMeasurement { Id = 5, Client = client, CreatorId = Guid.NewGuid(), Name = "updated", Timestamp = DateTime.Now, StartTime = DateTime.Now.AddDays(-99), EndTime = DateTime.Now };

            bool result = await timeDao.UpdateAsync(measurement);

            result.Should().BeTrue();
            (await timeDao.FindByIdAsync(measurement.Id)).Should().BeEquivalentTo(measurement, FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestUnsuccessfulUpdate()
        {
            // Measurement does not exit
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            TimeMeasurement measurement = new TimeMeasurement { Id = 23, Client = client, CreatorId = Guid.NewGuid(), Name = "updated", Timestamp = DateTime.Now, StartTime = DateTime.Now.AddDays(-99), EndTime = DateTime.Now };

            bool result = await timeDao.UpdateAsync(measurement);

            result.Should().BeFalse();

            // Client does not exist
            client.Id = 100;
            measurement.Id = 5;

            result = await timeDao.UpdateAsync(measurement);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task TestFindAll()
        {
            var times = await timeDao.FindAllAsync().ToListAsync();

            times.Count.Should().Be(3);
            times.Should().ContainSingle(l => l.Id == 5 && l.Client.Id == 1 && l.Name == "time1");
            times.Should().ContainSingle(l => l.Id == 6 && l.Client.Id == 2 && l.Name == "time2");
        }

        [Fact]
        public async Task TestFindByCreator()
        {
            Guid expectedGuid = Guid.Parse("09b87c32-d36a-4050-861a-244f86f754a8");
            var times = await timeDao.FindByCreatorAsync(2, expectedGuid).ToListAsync();

            times.Count.Should().Be(2);
            times.Should().OnlyContain(x => x.CreatorId == expectedGuid && x.Client.Id == 2);
        }

        [Fact]
        public async Task FindByName()
        {
            var times = await timeDao.FindAllByNameAsync(2, "time2").ToListAsync();

            times.Count.Should().Be(2);
            times.Should().OnlyContain(x => x.Name == "time2");
        }


        [Fact]
        public async Task TestFindOne()
        {
            (await timeDao.FindByIdAsync(-1)).Should().BeNull();

            var metric = await timeDao.FindByIdAsync(5);
            metric.Id.Should().Be(5);
            metric.Name.Should().Be("time1");
            metric.Client.Id.Should().Be(1);
            metric.EndTime.Should().BeCloseTo(DateTime.Now, 1.Seconds());
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            TimeMeasurement existingMeasurement = new TimeMeasurement { Id = 5 };
            (await timeDao.DeleteAsync(existingMeasurement)).Should().BeTrue();
            (await timeDao.FindByIdAsync(existingMeasurement.Id)).Should().BeNull();

            TimeMeasurement notExistingMeasurement = new TimeMeasurement { Id = 100 };
            (await timeDao.DeleteAsync(notExistingMeasurement)).Should().BeFalse();
        }
    }
}
