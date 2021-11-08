using AaaS.Dal.Ado;
using AaaS.Dal.Ado.Telementry;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
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
    public class TelemetryTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly ILogDao logDao;
        readonly IClientDao clientDao;

        public TelemetryTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            logDao = new MSSQLLogDao(this.fixture.ConnectionFactory);
            clientDao = new MSSQLClientDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsertLog()
        {
            Client client = new Client { Id=1, ApiKey = "customkey1", Name = "client1" };
            Log log = new Log { Client = client, CreatorId = Guid.NewGuid(), Message = "1", Name = "testname", Timestamp = DateTime.Now, Type = new LogType { Id = 1 } };

            await logDao.InsertAsync(log);

            log.Id.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task TestFindAll()
        {
            //(await logDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(ClientLi)
        }

        //[Fact]
        //[AutoRollback]
        //public async Task TestFindOne()
        //{
        //    Client client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" };
        //    Log log = new Log { Client = client, CreatorId = Guid.NewGuid(), Message = "1", Name = "testname", Timestamp = DateTime.Now, Type = new LogType { Id = 1, Name="Error" } };

        //    await logDao.InsertAsync(log);

        //    (await logDao.FindByIdAsync(log.Id)).Should().BeEquivalentTo(log, o => o
        //        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
        //        .WhenTypeIs<DateTime>()
        //    );


        //}
    }
}
