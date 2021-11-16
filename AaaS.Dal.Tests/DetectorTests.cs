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
using Xunit.Abstractions;

namespace AaaS.Dal.Tests
{
    [Collection("SeededDb")]
    public class DetectorTests : IClassFixture<DatabaseFixture>
    {
        readonly DatabaseFixture fixture;
        readonly IDetectorDao detectorDao;
        readonly IActionDao actionDao;
        readonly IClientDao clientDao;
        readonly IObjectPropertyDao objectPropertyDao;
        private readonly ITestOutputHelper output;

        public DetectorTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            detectorDao = new MSSQLDetectorDao(this.fixture.ConnectionFactory);
            actionDao = new MSSQLActionDao(this.fixture.ConnectionFactory);
            clientDao = new MSSQLClientDao(this.fixture.ConnectionFactory);
            objectPropertyDao = new MSSQLObjectPropertyDao(this.fixture.ConnectionFactory);
            this.output = output;
        }

        [Fact]
        public async Task TestFindById()
        {
            (await detectorDao.FindByIdAsync(3)).Should().BeEquivalentTo(DetectorList.First());
            (await detectorDao.FindByIdAsync(1)).Should().BeNull();
        }

        [Fact]
        public async Task TestFindAll()
        {
            output.WriteLine(new SimpleDetector().GetType().AssemblyQualifiedName);
            (await detectorDao.FindAllAsync().ToListAsync()).Should().BeEquivalentTo(DetectorList);
        }

        [Fact]
        [AutoRollback]
        public async Task TestDelete()
        {
            var detectorToDelete = new SimpleDetector { Id = 3, TelemetryName = "TestTelemetry1", Action = new SimpleAction { Id = 1 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" } };

            (await detectorDao.DeleteAsync(detectorToDelete)).Should().BeTrue();
            (await detectorDao.FindByIdAsync(3)).Should().BeNull();
            (await objectPropertyDao.FindByObjectIdAsync(3).ToListAsync()).Should().NotContain(x => x.ObjectId == 3);
            (await objectPropertyDao.FindAllAsync().ToListAsync()).Should().NotBeEmpty();

            var invalidDetector = new SimpleDetector { Id = 900, TelemetryName = "TestTelemetry1", Action = new SimpleAction { Id = 1 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" } };

            (await detectorDao.DeleteAsync(invalidDetector)).Should().BeFalse();
        }

        [Fact]
        [AutoRollback]
        public async Task TestUpdate()
        {
            var notExistingDetector = new SimpleDetector { Id = 100, TelemetryName = "TestTelemetry1", Action = new SimpleAction { Id = 1 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" } };
            (await detectorDao.UpdateAsync(notExistingDetector)).Should().BeFalse();

            var detectorWithExistingFKs = new SimpleDetector { Id = 3, TelemetryName = "TestTelemetryUpdated", Action = new SimpleAction { Id = 1 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" } };
            (await detectorDao.UpdateAsync(detectorWithExistingFKs)).Should().BeTrue();
            (await detectorDao.FindByIdAsync(3)).Should().BeEquivalentTo(detectorWithExistingFKs);
            
            var detectorWithNonExistingAction = new SimpleDetector { Id = 3, TelemetryName = "TestTelemetryUpdated", Action = new SimpleAction { Email = "abc", TemplateText = "def", Value = 12 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client { Id = 1, ApiKey = "customkey1", Name = "client1" } };
            (await detectorDao.UpdateAsync(detectorWithNonExistingAction)).Should().BeTrue();
            (await detectorDao.FindByIdAsync(3)).Should().BeEquivalentTo(detectorWithNonExistingAction);

            var detectorWithNonExistingClient = new SimpleDetector { Id = 3, TelemetryName = "TestTelemetryUpdated", Action = new SimpleAction { Id = 1 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client { ApiKey = "customkey11", Name = "client1" } };
            (await detectorDao.UpdateAsync(detectorWithNonExistingClient)).Should().BeTrue();
            (await detectorDao.FindByIdAsync(3)).Should().BeEquivalentTo(detectorWithNonExistingClient);
        }

        [Fact]
        [AutoRollback]
        public async Task TestInsert()
        {
            Detector completelyFreshDetector = new SimpleDetector
            {
                TelemetryName = "InsertTestTelemetry",
                Client = new Client { ApiKey = "DetectorInsertKey", Name = "Detector Insert Client" },
                Action = new SimpleAction { Email = "insert@mail.com", Value = 127, TemplateText = "This Action needs to be inserted first." },
                CheckInterval = TimeSpan.FromMilliseconds(1000),
                Name = "InsertTestDetector"
            };
            await detectorDao.InsertAsync(completelyFreshDetector);
            completelyFreshDetector.Id.Should().Be(4);
            (await detectorDao.FindByIdAsync(4)).Should().BeEquivalentTo(completelyFreshDetector);


            var actionCountBeforeInsert = await actionDao.FindAllAsync().CountAsync();
            var clientCountBeforeInsert = await clientDao.FindAllAsync().CountAsync();
            Detector detectorWithExistingAction = new SimpleDetector
            {
                TelemetryName = "InsertTelemetry",
                Action = (await actionDao.FindByIdAsync(1)),
                Client = (await clientDao.FindByIdAsync(1)),
                CheckInterval = TimeSpan.FromMilliseconds(1000),
                Name = "Test2"
            };
            await detectorDao.InsertAsync(detectorWithExistingAction);
            detectorWithExistingAction.Id.Should().Be(6);
            (await detectorDao.FindByIdAsync(6)).Should().BeEquivalentTo(detectorWithExistingAction);
            (await actionDao.FindAllAsync().CountAsync()).Should().Be(actionCountBeforeInsert);
            (await clientDao.FindAllAsync().CountAsync()).Should().Be(clientCountBeforeInsert);
        }

        public static IEnumerable<Detector> DetectorList => new List<Detector> {
                new SimpleDetector { Id=3, TelemetryName = "TestTelemetry1", Action = new SimpleAction{ Id = 1 }, CheckInterval = TimeSpan.FromMilliseconds(1000), Client = new Client{Id=1, ApiKey = "customkey1", Name="client1" } }};
    }
}
