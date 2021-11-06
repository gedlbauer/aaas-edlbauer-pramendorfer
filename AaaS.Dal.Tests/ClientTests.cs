using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
using AaaS.Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace AaaS.Dal.Tests
{
    public class ClientTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly IClientDao clientDao;

        public ClientTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            clientDao = new MSSQLClientDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsert()
        {
            Client client = new Client { ApiKey = "test-ksdfeydfsdfd", Name = "test-user" };

            await clientDao.InsertAsync(client);

            client.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        [AutoRollback]
        public async Task TestFindById()
        {
            Client client = new Client { ApiKey = "test1", Name = "test-user" };
            Client client2 = new Client { ApiKey = "test2", Name = "test-user2" };
            await clientDao.InsertAsync(client);
            await clientDao.InsertAsync(client2);

            (await clientDao.FindByIdAsync(-1)).Should().BeNull();
            (await clientDao.FindByIdAsync(client.Id)).Should().BeEquivalentTo(client);
            (await clientDao.FindByIdAsync(client2.Id)).Should().BeEquivalentTo(client2);
        }
    }
}
