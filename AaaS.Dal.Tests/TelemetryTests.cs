using AaaS.Dal.Ado;
using AaaS.Dal.Ado.Telemetry;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
using AaaS.Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace AaaS.Dal.Tests
{
    [Collection("SeededDb")]
    public class TelemetryTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly ILogDao logDao;

        public TelemetryTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            logDao = new MSSQLLogDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsertLog()
        {
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            Log log = new Log { Client = client, CreatorId = Guid.NewGuid(), Message = "1", Name = "testname", Timestamp = DateTime.Now, Type = new LogType { Id = 1 } };

            await logDao.InsertAsync(log);

            log.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        // TODO ensure that update does not interfere with find tests
        public async Task TestUpdate()
        {
            Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
            Log log = new Log { Id = 1, Client = client, CreatorId = Guid.NewGuid(), Message = "updated", Name = "testname", Timestamp = DateTime.Now, Type = new LogType { Id = 1, Name = "Error" } };

            await logDao.UpdateAsync(log);

            (await logDao.FindByIdAsync(log.Id)).Should().BeEquivalentTo(log, o => o
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
        }

        [Fact]
        public async Task TestFindAll()
        {
            var logs = await logDao.FindAllAsync().ToListAsync();

            logs.Count.Should().Be(2);
            logs.Should().ContainSingle(l => l.Id == 1 && l.Client.Id == 1 && l.Name == "log1");
            logs.Should().ContainSingle(l => l.Id == 2 && l.Client.Id == 2 && l.Name == "log2");
        }

        [Fact]

        public async Task TestFindOne()
        {
            (await logDao.FindByIdAsync(-1)).Should().BeNull();

            var log = await logDao.FindByIdAsync(1);
            log.Id.Should().Be(1);
            log.Name.Should().Be("log1");
            log.Client.Id.Should().Be(1);
            log.Type.Should().BeEquivalentTo(new LogType { Id = 1, Name = "Error" });
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            throw new NotImplementedException("Bitte implementieren!");
            //TODO: delete + test implementieren
        }
    }
}
