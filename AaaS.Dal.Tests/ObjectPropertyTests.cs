using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.Attributes;
using AaaS.Dal.Tests.Infrastructure;
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
    public class ObjectPropertyTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly IObjectPropertyDao objectPropertyDao;

        public ObjectPropertyTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            objectPropertyDao = new MSSQLObjectPropertyDao(this.fixture.ConnectionFactory);
        }

        [Fact]
        public async Task TestFindByObjectIdAndName()
        {
            (await objectPropertyDao.FindByObjectIdAndNameAsync(1, "Email")).Should().BeEquivalentTo(PropertyList.First());
            (await objectPropertyDao.FindByObjectIdAndNameAsync(-1, "Email")).Should().BeNull();
            (await objectPropertyDao.FindByObjectIdAndNameAsync(1, "NotExisting")).Should().BeNull();
        }

        [Fact]
        public async Task TestFindByObjectId()
        {
            (await objectPropertyDao.FindByObjectIdAsync(1).ToListAsync()).Should().BeEquivalentTo(PropertyList.Where(x => x.ObjectId == 1));
        }

        [Fact]
        public async Task TestFindAll()
        {
            (await objectPropertyDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(PropertyList);
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            var updatedProperty = new ObjectProperty { ObjectId = 1, Name = "Email", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value = "updatedtestmail@test.com" };
            (await objectPropertyDao.UpdateAsync(updatedProperty)).Should().BeTrue();
            (await objectPropertyDao.FindByObjectIdAndNameAsync(1, "Email")).Should().BeEquivalentTo(updatedProperty);

            var invalidUpdateProperty = new ObjectProperty { ObjectId = 21, Name = "Email", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value = "updatedtestmail@test.com" };
            (await objectPropertyDao.UpdateAsync(invalidUpdateProperty)).Should().BeFalse();
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsert()
        {
            var propToInsert = new ObjectProperty { ObjectId = 1, Name = "TestProp", TypeName = "TestTypeName", Value = "TestValue" };
            await objectPropertyDao.InsertAsync(propToInsert);
            (await objectPropertyDao.FindByObjectIdAndNameAsync(1, "TestProp")).Should().BeEquivalentTo(propToInsert);
            var invalidProp = new ObjectProperty { ObjectId = -1, Name = "TestProp", TypeName = "TestTypeName", Value = "TestValue" };
            await objectPropertyDao.Invoking(async x => await objectPropertyDao.InsertAsync(invalidProp)).Should().ThrowAsync<SqlException>();
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            var propToDelete = new ObjectProperty { ObjectId = 1, Name = "Email", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value = "\"testmail@test.com\"" };
            (await objectPropertyDao.DeleteAsync(propToDelete)).Should().BeTrue();
            (await objectPropertyDao.FindByObjectIdAndNameAsync(1, "TestProp")).Should().BeNull();
            (await objectPropertyDao.FindAllAsync().ToListAsync()).Should().NotBeEmpty();

            var notExistingProp = new ObjectProperty { ObjectId = 1, Name = "TestProp", TypeName = "TestTypeName", Value = "TestValue" };
            (await objectPropertyDao.DeleteAsync(notExistingProp)).Should().BeFalse();
        }

        public IEnumerable<ObjectProperty> PropertyList = new List<ObjectProperty>
        {
            new ObjectProperty{ObjectId = 1, Name = "Email", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value="\"testmail@test.com\""},
            new ObjectProperty{ObjectId = 1, Name = "TemplateText", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value="\"Das ist eine Testmail\""},
            new ObjectProperty{ObjectId = 1, Name = "Value", TypeName = "System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value="12"},
            new ObjectProperty{ObjectId = 2, Name = "Email", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value="\"testmail@test.com\""},
            new ObjectProperty{ObjectId = 2, Name = "TemplateText", TypeName = "System.String, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value="\"Das ist eine Testmail\""},
            new ObjectProperty{ObjectId = 2, Name = "Value", TypeName = "System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", Value="12"}
        };
    }
}
