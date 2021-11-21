using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
using AaaS.Dal.Tests.Infrastructure;
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
    [Collection("SeededDb")]
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
            Client client = new Client { ApiKey = "test-key-test", Name = "test-user" };

            await clientDao.InsertAsync(client);

            client.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            await clientDao.DeleteAsync(new Client { Id = 3 });

            (await clientDao.FindByIdAsync(3)).Should().BeNull();
        }

        [Fact]
        [AutoRollback]
        public async Task TestSuccessfullUpdate()
        {
            Client client = ClientList.First();
            client.ApiKey = "new-key";
            client.Name = "updated-name";

           bool result = await clientDao.UpdateAsync(client);

            result.Should().BeTrue();
            (await clientDao.FindByIdAsync(client.Id)).Should().BeEquivalentTo(client);
        }

        [Fact]
        [AutoRollback]
        public async Task TestFailingUpdate()
        {
            Client nonExistingClient = ClientList.First();
            nonExistingClient.Id = 100;

            bool result = await clientDao.UpdateAsync(nonExistingClient);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task TestFindById()
        {
            (await clientDao.FindByIdAsync(-1)).Should().BeNull();
            (await clientDao.FindByIdAsync(ClientList.First().Id)).Should().BeEquivalentTo(ClientList.First());
        }

        [Fact]
        public async Task TestFindAll()
        {
            (await clientDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(ClientList);
        }

        public static IEnumerable<Client> ClientList
            => new List<Client> {
                new Client { Id=1, ApiKey = "customkey1", Name = "client1" },
                new Client { Id=2, ApiKey = "customkey2", Name = "client2" },
                new Client { Id=3, ApiKey = "customkey3", Name = "client3" }};

    }
}
