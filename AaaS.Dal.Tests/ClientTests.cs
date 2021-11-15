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
        public async Task TestUpdate()
        {
            Client client = ClientList.First();
            client.ApiKey = "new-key";
            client.Name = "updated-name";

            await clientDao.UpdateAsync(client);

            (await clientDao.FindByIdAsync(client.Id)).Should().BeEquivalentTo(client);
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

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            throw new NotImplementedException("Bitte Implementieren!");
            //TODO: delete + test implementieren
        }

        public static IEnumerable<Client> ClientList
            => new List<Client> {
                new Client { Id=1, ApiKey = "customkey1", Name = "client1" },
                new Client { Id=2, ApiKey = "customkey2", Name = "client2" } };

    }
}
