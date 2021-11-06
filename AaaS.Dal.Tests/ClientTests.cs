using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task TestInsert()
        {
            Client client = new Client { ApiKey = "test-keysdfd", Name = "test-user" };
            await clientDao.InsertAsync(client);
            (await clientDao.FindByIdAsync(client.Id)).Should().BeEquivalentTo(client);
        }
    }
}
