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
            Client client = new Client { ApiKey = "test-key-test", Name = "test-user" };

            await clientDao.InsertAsync(client);

            client.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            Client client = new Client { ApiKey = "test-key-test", Name = "test-user" };
            await clientDao.InsertAsync(client);
            client.ApiKey = "new-key";
            client.Name = "updated-name";

            await clientDao.UpdateAsync(client);

            (await clientDao.FindByIdAsync(client.Id)).Should().BeEquivalentTo(client);
        }

        [Fact]
        [AutoRollback]
        public async Task TestFindById()
        {
            var clients = await SeedClients(ClientList);

            (await clientDao.FindByIdAsync(-1)).Should().BeNull();
            foreach (var client in clients)
            {
                (await clientDao.FindByIdAsync(client.Id)).Should().BeEquivalentTo(client);
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        [AutoRollback]
        public async Task TestFindAll(List<Client> clients)
        {
            await SeedClients(clients);

            (await clientDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(clients);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]> {
                new object[]{ClientList},
                new object[]{new List<Client> { }}
            };

        public static IEnumerable<Client> ClientList
            => new List<Client> {
                new Client { ApiKey = "test-1", Name = "test-user" },
                new Client { ApiKey = "test-2", Name = "test-user2" } };


        private async Task<IEnumerable<Client>> SeedClients(IEnumerable<Client> clients)
        {
            foreach (var client in clients)
            {
                await clientDao.InsertAsync(client);
            }
            return clients;
        }
    }
}
