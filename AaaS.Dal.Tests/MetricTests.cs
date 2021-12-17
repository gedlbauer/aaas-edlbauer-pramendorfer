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
    public class MetricTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly IMetricDao metricDao;

        public MetricTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            metricDao = new MSSQLMetricDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsertMetric()
        {
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            Metric metric = new Metric { Client = client, CreatorId = Guid.NewGuid(), Name = "testname", Timestamp = DateTime.Now, Value=1 };

            await metricDao.InsertAsync(metric);

            metric.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            Metric metric = new Metric { Id = 3, Client = client, CreatorId = Guid.NewGuid(), Name = "testname", Timestamp = DateTime.Now, Value = 100 };
           
            await metricDao.UpdateAsync(metric);

            (await metricDao.FindByIdAsync(metric.Id)).Should().BeEquivalentTo(metric, FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindAll()
        {
            var metrics = await metricDao.FindAllAsync().ToListAsync();

            metrics.Count.Should().Be(2);
            metrics.Should().ContainSingle(l => l.Id == 3 && l.Client.Id == 1 && l.Name == "metric1");
            metrics.Should().ContainSingle(l => l.Id ==4 && l.Client.Id == 2 && l.Name == "metric2");
        }

        [Fact]
        public async Task TestFindAllSince()
        {
            var metrics = await metricDao.FindSinceByClientAndTelemetryNameAsync(DateTime.Now.AddDays(-1), 1, "metric1").ToListAsync();

            metrics.Count.Should().Be(1);
            metrics.Should().ContainSingle(m => m.Client.Id == 1 && m.Name == "metric1");

            (await metricDao.FindSinceByClientAndTelemetryNameAsync(DateTime.Now, 1, "metric1").ToListAsync()).Count.Should().Be(0);
        }

        [Fact]
        public async Task TestFindOne()
        {
            (await metricDao.FindByIdAsync(-1)).Should().BeNull();

            var metric = await metricDao.FindByIdAsync(3);
            metric.Id.Should().Be(3);
            metric.Name.Should().Be("metric1");
            metric.Client.Id.Should().Be(1);
            metric.Value.Should().Be(10);
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            Metric log = new Metric { Id = 3 };

            await metricDao.DeleteAsync(log);

            (await metricDao.FindByIdAsync(log.Id)).Should().BeNull();
        }
    }
}
