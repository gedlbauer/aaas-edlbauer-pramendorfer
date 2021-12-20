using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Core.Exceptions;
using AaaS.Core.Managers;
using AaaS.Core.Repositories;
using AaaS.Dal.Interface;
using AaaS.Dal.Tests.TestObjects;
using AaaS.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Managers
{
    public class DetectorManagerTests
    {

        private static List<Client> clients = new List<Client>
        {
            new Client
            {
                ApiKey = "xxx",
                Id = 2,
                Name = "Client 2"
            },
            new Client
            {
                ApiKey = "xxx",
                Id = 3,
                Name = "Client 3"
            }
        };

        private static List<BaseAction> actions = new List<BaseAction>
        {
            new WebHookAction
                {
                    Id = 1,
                    Name = "Test Action",
                    RequestUrl = "test.me",
                    Client = clients[0]
                },
            new WebHookAction
                {
                    Id = 2,
                    Name = "Test Action",
                    RequestUrl = "test.me",
                    Client = clients[1]
                }
        };

        private static List<BaseDetector> detectors = new List<BaseDetector>
        {
            new SimpleDetector
            {
                Id = 1,
                CheckInterval = TimeSpan.FromSeconds(1),
                Client = clients[0],
                Action = actions[0],
                TelemetryName = "Test Telemetry",
                Name = "x"
            },
            new SimpleDetector
            {
                Id = 2,
                CheckInterval = TimeSpan.FromSeconds(1),
                Client = clients[1],
                Action = actions[1],
                TelemetryName = "Test Telemetry 2",
                Name = "x2"
            }
        };

        private static Mock<IDetectorDao<BaseDetector, BaseAction>> GetDefaultDetectorDaoMock()
        {
            var detectorDaoMock = new Mock<IDetectorDao<BaseDetector, BaseAction>>();
            detectorDaoMock.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns<int>(id => Task.FromResult(detectors.SingleOrDefault(x => x.Id == id)));
            detectorDaoMock.Setup(x => x.FindAllAsync()).Returns(detectors.ToAsyncEnumerable());
            return detectorDaoMock;
        }

        private static Mock<IClientDao> GetDefaultClientDaoMock()
        {
            var clientMock = new Mock<IClientDao>();
            clientMock.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns<int>(id => Task.FromResult(clients.SingleOrDefault(x => x.Id == id)));
            clientMock.Setup(x => x.FindAllAsync()).Returns(clients.ToAsyncEnumerable());
            return clientMock;
        }

        private static Mock<IActionManager> GetDefaultActionManagerMock()
        {
            var actionManagerMock = new Mock<IActionManager>();
            actionManagerMock.Setup(x => x.GetAll()).Returns(actions);
            actionManagerMock.Setup(x => x.GetAllFromClient(It.IsAny<int>())).Returns(actions);
            actionManagerMock.Setup(x => x.FindActionById(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((clientId, id) => actions.SingleOrDefault(x => x.Id == id && x.Client.Id == clientId));
            actionManagerMock.Setup(x => x.FindActionById(It.IsAny<int>())).Returns<int>(id => actions.SingleOrDefault(x => x.Id == id));
            return actionManagerMock;
        }
        private static Mock<IMetricRepository> GetDefaultMetricRepositoryMock()
        {
            var metricRepoMock = new Mock<IMetricRepository>();
            metricRepoMock.Setup(x => x.FindAllAsync(It.IsAny<int>()));
            metricRepoMock.Setup(x => x.FindSinceByClientAndTelemetryNameAsync(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<Metric>().ToAsyncEnumerable());
            return metricRepoMock;
        }

        public class GetTests
        {
            [Fact]
            public void TestFindById()
            {
                BaseDetector expected = detectors.SingleOrDefault(x => x.Id == 1);
                var manager = new DetectorManager(GetDefaultDetectorDaoMock().Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                Assert.Equal(expected, manager.FindDetectorById(1));
            }

            [Fact]
            public void TestFindByIdWithInvalidId()
            {
                BaseDetector expected = null;
                var manager = new DetectorManager(GetDefaultDetectorDaoMock().Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                Assert.Equal(expected, manager.FindDetectorById(4));
            }

            [Theory]
            [InlineData(2, 1)]
            [InlineData(4, 1)]
            [InlineData(2, 10)]
            [InlineData(20, 10)]
            public void TestFindByIdAndClientId(int clientId, int id)
            {
                BaseDetector expected = detectors.SingleOrDefault(x => x.Id == id && x.Client.Id == clientId);
                var manager = new DetectorManager(GetDefaultDetectorDaoMock().Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                Assert.Equal(expected, manager.FindDetectorById(clientId, id));
            }

            [Fact]
            public void TestGetAll()
            {
                var expected = detectors;
                var manager = new DetectorManager(GetDefaultDetectorDaoMock().Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);

                var result = manager.GetAll();

                Assert.Equal(expected, result);
            }

            [Theory]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            public void TestGetAllFromClient(int clientId)
            {
                var expected = detectors.Where(x => x.Client.Id == clientId);
                var manager = new DetectorManager(GetDefaultDetectorDaoMock().Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);

                Assert.Equal(expected, manager.GetAllFromClient(clientId));
            }

            public class AddTests
            {
                [Fact]
                public async Task TestInsert()
                {
                    var detectorToInsert = new SimpleDetector
                    {
                        Name = "Inserted Detector",
                        Action = actions[0],
                        TelemetryName = "Insert Detector Test",
                        CheckInterval = TimeSpan.FromSeconds(1000),
                        Client = new Client
                        {
                            ApiKey = "xxx",
                            Id = 2,
                            Name = "Client 2"
                        }
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await manager.AddAndStartDetectorAsync(detectorToInsert);
                    detectorDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseDetector>()), Times.Once());
                }

                [Fact]
                public async Task TestInsertWithNullClient()
                {
                    var detectorToInsert = new SimpleDetector
                    {
                        Name = "Inserted Detector",
                        Action = actions[0],
                        CheckInterval = TimeSpan.FromSeconds(100)
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await Assert.ThrowsAsync<ArgumentException>(async () => await manager.AddAndStartDetectorAsync(detectorToInsert));
                    detectorDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseDetector>()), Times.Never());
                }

                [Fact]
                public async Task TestInsertWithInvalidClient()
                {
                    var detectorToInsert = new SimpleDetector
                    {
                        Name = "Inserted Detector",
                        Action = new WebHookAction
                        {
                            Client = new Client
                            {
                                ApiKey = "xxx",
                                Id = 0,
                                Name = "Client 3"
                            }
                        },
                        CheckInterval = TimeSpan.FromSeconds(100),
                        Client = new Client
                        {
                            ApiKey = "xxx",
                            Id = 0,
                            Name = "Client 3"
                        }
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await Assert.ThrowsAsync<ArgumentException>(async () => await manager.AddAndStartDetectorAsync(detectorToInsert));
                    detectorDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseDetector>()), Times.Never());
                }

                [Fact]
                public async Task TestInsertWithConflictingActionClient()
                {
                    var detectorToInsert = new SimpleDetector
                    {
                        Name = "Inserted Detector",
                        Action = actions[0],
                        CheckInterval = TimeSpan.FromSeconds(100),
                        Client = clients[1]
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await Assert.ThrowsAsync<AaaSAuthorizationException>(async () => await manager.AddAndStartDetectorAsync(detectorToInsert));
                    detectorDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseDetector>()), Times.Never());
                }
            }

            public class UpdateTests
            {
                [Fact]
                public async Task TestUpdate()
                {
                    var detectorToUpdate = new SimpleDetector
                    {
                        Id = 1,
                        CheckInterval = TimeSpan.FromSeconds(1),
                        Client = clients[0],
                        Action = actions[0],
                        TelemetryName = "Test Telemetry with updated Telemetry Name",
                        Name = "Also with new Name"
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var actionManagerMock = GetDefaultActionManagerMock();
                    actionManagerMock.Setup(x => x.AddActionAsync(It.IsAny<BaseAction>())).Verifiable();

                    var manager = new DetectorManager(detectorDaoMock.Object, actionManagerMock.Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await manager.UpdateDetectorAsync(detectorToUpdate);
                    detectorDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseDetector>()), Times.Once());
                    actionManagerMock.Verify(x => x.AddActionAsync(It.IsAny<BaseAction>()), Times.Never());
                }

                [Fact]
                public async Task TestUpdateWithNewAction()
                {
                    var detectorToUpdate = new SimpleDetector
                    {
                        Id = 1,
                        CheckInterval = TimeSpan.FromSeconds(1),
                        Client = clients[0],
                        Action = new WebHookAction
                        {
                            Client = clients[0],
                            Name = "Test Action tmp",
                            RequestUrl = "tst.tmp"
                        },
                        TelemetryName = "Test Telemetry2",
                        Name = "xasdf"
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var actionManagerMock = GetDefaultActionManagerMock();
                    actionManagerMock.Setup(x => x.AddActionAsync(It.IsAny<BaseAction>())).Verifiable();

                    var manager = new DetectorManager(detectorDaoMock.Object, actionManagerMock.Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await manager.UpdateDetectorAsync(detectorToUpdate);
                    detectorDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseDetector>()), Times.Once());
                    actionManagerMock.Verify(x => x.AddActionAsync(It.IsAny<BaseAction>()), Times.Once());
                }

                [Fact]
                public async Task TestUpdateWithInvalidClient()
                {
                    var detectorToUpdate = new SimpleDetector
                    {
                        Id = 1,
                        CheckInterval = TimeSpan.FromSeconds(1),
                        Client = clients[0],
                        Action = actions[1],
                        TelemetryName = "Test Telemetry2",
                        Name = "xasdf"
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await Assert.ThrowsAsync<AaaSAuthorizationException>(async () => await manager.UpdateDetectorAsync(detectorToUpdate));
                    detectorDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseDetector>()), Times.Never());
                }

                [Fact]
                public async Task TestUpdateWithNullClient()
                {
                    var detectorToUpdate = new SimpleDetector
                    {
                        Id = 1,
                        CheckInterval = TimeSpan.FromSeconds(1),
                        Client = null,
                        Action = actions[1],
                        TelemetryName = "Test Telemetry2",
                        Name = "xasdf"
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await Assert.ThrowsAsync<ArgumentException>(async () => await manager.UpdateDetectorAsync(detectorToUpdate));
                    detectorDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseDetector>()), Times.Never());
                }

                [Fact]
                public async Task TestUpdateWithoutId()
                {
                    var detectorToUpdate = new SimpleDetector
                    {
                        CheckInterval = TimeSpan.FromSeconds(1),
                        Client = clients[0],
                        Action = new WebHookAction
                        {
                            Client = clients[0],
                            Name = "Test Action tmp",
                            RequestUrl = "tst.tmp"
                        },
                        TelemetryName = "Test Telemetry2",
                        Name = "xasdf"
                    };

                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await Assert.ThrowsAsync<ArgumentException>(async () => await manager.UpdateDetectorAsync(detectorToUpdate));
                    detectorDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseDetector>()), Times.Never());
                }
            }


            public class DeleteTests
            {
                [Fact]
                public async Task TestDelete()
                {
                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.DeleteAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await manager.DeleteDetectorAsync(detectors[0]);
                    detectorDaoMock.Verify(x => x.DeleteAsync(It.IsAny<BaseDetector>()), Times.Once());
                }

                [Fact]
                public async Task TestDeleteWithNull()
                {
                    var detectorDaoMock = GetDefaultDetectorDaoMock();
                    detectorDaoMock.Setup(x => x.DeleteAsync(It.IsAny<BaseDetector>())).Verifiable();
                    var manager = new DetectorManager(detectorDaoMock.Object, GetDefaultActionManagerMock().Object, GetDefaultClientDaoMock().Object, GetDefaultMetricRepositoryMock().Object);
                    await manager.DeleteDetectorAsync(null);
                    detectorDaoMock.Verify(x => x.DeleteAsync(It.IsAny<BaseDetector>()), Times.Never());
                }
            }
        }
    }
}
