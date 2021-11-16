﻿using AaaS.Core;
using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
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
        readonly IActionDao actionDao;
        readonly IObjectPropertyDao objectPropertyDao;

        public ActionTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            actionDao = new MSSQLActionDao(this.fixture.ConnectionFactory);
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
            (await actionDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(ActionList);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsert()
        {
            var insertAction = new SimpleAction { Email = "test@testmail.com", TemplateText = "Das ist eine Testmail", Value = 9001 };
            await actionDao.InsertAsync(insertAction);
            insertAction.Id.Should().BeGreaterThan(0);
            (await actionDao.FindByIdAsync(insertAction.Id)).Should().BeEquivalentTo(insertAction);
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            var actionToUpdate = new SimpleAction { Email = "test@testmail.com", TemplateText = "Das ist eine veränderte Testaction", Id = 1, Value = 40 };
            (await actionDao.UpdateAsync(actionToUpdate)).Should().BeTrue();
            (await actionDao.FindByIdAsync(1)).Should().BeEquivalentTo(actionToUpdate);
            var invalidActionToUpdate = new SimpleAction { Email = "test@testmail.com", TemplateText = "Das ist eine veränderte Testaction", Id = -1, Value = 40 };

            var actionToUpdate2 = new SimpleAction { Email = null, TemplateText = "Das ist eine veränderte Testaction", Id = 1, Value = 40 };
            (await actionDao.UpdateAsync(actionToUpdate2)).Should().BeTrue();
            (await actionDao.FindByIdAsync(1)).Should().BeEquivalentTo(actionToUpdate2);

            await actionDao.Invoking(async x => await actionDao.UpdateAsync(invalidActionToUpdate)).Should().ThrowAsync<SqlException>();
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            var fkDependentAction = new SimpleAction { Id = 1, Email = "testmail@test.com", TemplateText = "Das ist eine Testmail", Value = 12 };
            await actionDao.Invoking(async x => await actionDao.DeleteAsync(fkDependentAction)).Should().ThrowAsync<SqlException>();

            var actionToDelete = new SimpleAction { Id = 2, Email = "testmail@test.com", TemplateText = "Das ist eine Testmail", Value = 12 };
            (await actionDao.DeleteAsync(actionToDelete)).Should().BeTrue();
            (await actionDao.FindByIdAsync(2)).Should().BeNull();
            (await objectPropertyDao.FindByObjectIdAsync(1).ToListAsync()).Should().NotContain(x => x.ObjectId == 2);
            (await objectPropertyDao.FindAllAsync().ToListAsync()).Should().NotBeEmpty();

            var invalidAction = new SimpleAction { Id = 3, Email = "testmail@test.com", TemplateText = "Das ist eine Testmail", Value = 12 };
            (await actionDao.DeleteAsync(invalidAction)).Should().BeFalse();

        }

        public static IEnumerable<IAction> ActionList => new List<IAction> {
                new SimpleAction { Id = 1, Email="testmail@test.com", TemplateText="Das ist eine Testmail", Value=12 },
                new SimpleAction { Id = 2, Email="testmail@test.com", TemplateText="Das ist eine Testmail", Value=12 }
        };
    }
}