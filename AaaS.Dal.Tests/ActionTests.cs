using AaaS.Core;
using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
using AaaS.Dal.Tests.Infrastructure;
using AaaS.Dal.Tests.TestObjects;
using AaaS.Domain;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Dal.Tests
{
    [Collection("SeededDb")]
    public class ActionTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly IActionDao<AaaSAction> actionDao;
        readonly IObjectPropertyDao objectPropertyDao;

        public ActionTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            actionDao = new MSSQLActionDao<AaaSAction>(this.fixture.ConnectionFactory);
            objectPropertyDao = new MSSQLObjectPropertyDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        public async Task TestFindById()
        {
            var action = await actionDao.FindByIdAsync(1);
            action.Should().BeEquivalentTo(ActionList.First());
            (await actionDao.FindByIdAsync(-1)).Should().BeNull();
        }

        [Fact]
        public async Task TestFindAll()
        {
            var type = typeof(SimpleAction).AssemblyQualifiedName;
            (await actionDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(ActionList);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsert()
        {
            var insertAction = new SimpleAction { Name = "Inserted1", Email = "test@testmail.com", Client = new Client { ApiKey = "apiX", Name = "ClientX" }, TemplateText = "Das ist eine Testmail", Value = 9001 };
            await actionDao.InsertAsync(insertAction);
            insertAction.Id.Should().BeGreaterThan(0);
            (await actionDao.FindByIdAsync(insertAction.Id)).Should().BeEquivalentTo(insertAction);
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            var notExistingAction = new SimpleAction { Id = 100, Name = "Simple100", Email = "testmail@test.com", Client = new Client { ApiKey = "customkey1", Id = 1, Name = "client1" }, TemplateText = "Das ist eine Testmail", Value = 12 };
            (await actionDao.UpdateAsync(notExistingAction)).Should().BeFalse();

            var actionToUpdate = new SimpleAction { Email = "test@testmail.com", Name = "New123", Client = new Client { ApiKey = "customkey1", Id = 1, Name = "client1" }, TemplateText = "Das ist eine veränderte Testaction", Id = 1, Value = 40 };
            (await actionDao.UpdateAsync(actionToUpdate)).Should().BeTrue();
            (await actionDao.FindByIdAsync(1)).Should().BeEquivalentTo(actionToUpdate);

            var actionToUpdate2 = new SimpleAction { Email = null, Name = "New456", Client = new Client { ApiKey = "customkey1", Id = 1, Name = "client1" }, TemplateText = "Das ist eine veränderte Testaction", Id = 1, Value = 40 };
            (await actionDao.UpdateAsync(actionToUpdate2)).Should().BeTrue();
            (await actionDao.FindByIdAsync(1)).Should().BeEquivalentTo(actionToUpdate2);
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            var fkDependentAction = new SimpleAction { Id = 1, Email = "testmail@test.com", Client = new Client { ApiKey = "api1", Id = 1, Name = "Client1" }, TemplateText = "Das ist eine Testmail", Value = 12 };
            await actionDao.Invoking(async x => await actionDao.DeleteAsync(fkDependentAction)).Should().ThrowAsync<SqlException>();

            var actionToDelete = new SimpleAction { Id = 2, Email = "testmail@test.com", Client = new Client { ApiKey = "api2", Id = 2, Name = "Client2" }, TemplateText = "Das ist eine Testmail", Value = 12 };
            (await actionDao.DeleteAsync(actionToDelete)).Should().BeTrue();
            (await actionDao.FindByIdAsync(2)).Should().BeNull();
            (await objectPropertyDao.FindByObjectIdAsync(1).ToListAsync()).Should().NotContain(x => x.ObjectId == 2);
            (await objectPropertyDao.FindAllAsync().ToListAsync()).Should().NotBeEmpty();

            var invalidAction = new SimpleAction { Id = 3, Email = "testmail@test.com", Client = new Client { ApiKey = "api3", Id = 3, Name = "Client3" }, TemplateText = "Das ist eine Testmail", Value = 12 };
            (await actionDao.DeleteAsync(invalidAction)).Should().BeFalse();

        }

        public static IEnumerable<AaaSAction> ActionList => new List<AaaSAction> {
                new SimpleAction { Id = 1, Name="Simple1", Client=new Client{ ApiKey="customkey1", Id = 1, Name="client1" }, Email="testmail@test.com", TemplateText="Das ist eine Testmail", Value=12 },
                new SimpleAction { Id = 2, Name="Simple2", Client=new Client{ ApiKey="customkey2", Id = 2, Name="client2" }, Email="testmail@test.com", TemplateText="Das ist eine Testmail", Value=12 }
        };
    }
}
