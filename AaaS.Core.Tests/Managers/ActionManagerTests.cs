using AaaS.Core.Actions;
using AaaS.Core.Managers;
using AaaS.Dal.Interface;
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
    public class ActionManagerTests
    {
        private static readonly List<BaseAction> actions = new List<BaseAction>
        {
            new WebHookAction
                {
                    Id = 1,
                    Name = "Test Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey="xxx",
                        Id = 2,
                        Name = "Client 2"
                    }
                },
            new WebHookAction
                {
                    Id = 2,
                    Name = "Test Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey="xxx",
                        Id = 3,
                        Name = "Client 3"
                    }
                }
        };

        private static Mock<IActionDao<BaseAction>> GetDefaultActionDaoMock()
        {
            var actionDaoMock = new Mock<IActionDao<BaseAction>>();
            actionDaoMock.Setup(x => x.FindByIdAsync(It.IsAny<int>())).Returns<int>(id => Task.FromResult(actions.SingleOrDefault(x => x.Id == id)));
            actionDaoMock.Setup(x => x.FindAllAsync()).Returns(actions.ToAsyncEnumerable());
            return actionDaoMock;
        }

        public class GetTests
        {
            [Fact]
            public void TestFindById()
            {
                BaseAction expected = actions.SingleOrDefault(x => x.Id == 1);
                var actionDaoMock = GetDefaultActionDaoMock();
                var manager = new ActionManager(actionDaoMock.Object, null);
                Assert.Equal(expected, manager.FindActionById(1));
            }

            [Fact]
            public void TestFindByIdWithInvalidId()
            {
                BaseAction expected = null;
                var actionDaoMock = GetDefaultActionDaoMock();
                var manager = new ActionManager(actionDaoMock.Object, null);
                Assert.Equal(expected, manager.FindActionById(4));
            }

            [Theory]
            [InlineData(2, 1)]
            [InlineData(4, 1)]
            [InlineData(2, 10)]
            [InlineData(20, 10)]
            public void TestFindByIdAndClientId(int clientId, int id)
            {
                BaseAction expected = actions.SingleOrDefault(x => x.Id == id && x.Client.Id == clientId);
                var actionDaoMock = GetDefaultActionDaoMock();
                var manager = new ActionManager(actionDaoMock.Object, null);
                Assert.Equal(expected, manager.FindActionById(clientId, id));
            }

            [Fact]
            public void TestFindAll()
            {
                var expected = actions;
                var actionDaoMock = GetDefaultActionDaoMock();
                var manager = new ActionManager(actionDaoMock.Object, null);
                Assert.Equal(expected, manager.GetAll());
            }

            [Theory]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            public void TestFindAllFromClient(int clientId)
            {
                var expected = actions.Where(x => x.Client.Id == clientId);
                var actionDaoMock = GetDefaultActionDaoMock();
                var manager = new ActionManager(actionDaoMock.Object, null);
                Assert.Equal(expected, manager.GetAllFromClient(clientId));
            }
        }

        public class AddTests
        {
            [Fact]
            public async Task TestInsert()
            {
                var actionToInsert = new WebHookAction
                {
                    Name = "Inserted Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey = "xxx",
                        Id = 2,
                        Name = "Client 2"
                    }
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await manager.AddActionAsync(actionToInsert);
                actionDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseAction>()), Times.Once());
            }

            [Fact]
            public async Task TestInsertWithNullClient()
            {
                var actionToInsert = new WebHookAction
                {
                    Name = "Inserted Action",
                    RequestUrl = "test.me"
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await Assert.ThrowsAsync<ArgumentException>(async () => await manager.AddActionAsync(actionToInsert));
                actionDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseAction>()), Times.Never());
            }

            [Fact]
            public async Task TestInsertWithInvalidClient()
            {
                var actionToInsert = new WebHookAction
                {
                    Name = "Inserted Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey = "xxx",
                        Id = 0,
                        Name = "Client 3"
                    }
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.InsertAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await Assert.ThrowsAsync<ArgumentException>(async () => await manager.AddActionAsync(actionToInsert));
                actionDaoMock.Verify(x => x.InsertAsync(It.IsAny<BaseAction>()), Times.Never());
            }
        }

        public class UpdateTests
        {
            [Fact]
            public async Task TestUpdate()
            {
                var actionToUpdate = new WebHookAction
                {
                    Id = 1,
                    Name = "Updated Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey = "xxx",
                        Id = 3,
                        Name = "Client 2"
                    }
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await manager.UpdateActionAsync(actionToUpdate);
                actionDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseAction>()), Times.Once());
            }

            [Fact]
            public async Task TestUpdateWithNullClient()
            {
                var actionToUpdate = new WebHookAction
                {
                    Id = 1,
                    Name = "Updated Action",
                    RequestUrl = "test.me"
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await Assert.ThrowsAsync<ArgumentException>(async () => await manager.UpdateActionAsync(actionToUpdate));
                actionDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseAction>()), Times.Never());
            }


            [Fact]
            public async Task TestUpdateWithInvalidClient()
            {
                var actionToUpdate = new WebHookAction
                {
                    Id = 1,
                    Name = "Updated Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey = "xxx",
                        Id = 0,
                        Name = "Client 3"
                    }
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await Assert.ThrowsAsync<ArgumentException>(async () => await manager.UpdateActionAsync(actionToUpdate));
                actionDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseAction>()), Times.Never());
            }
            [Fact]
            public async Task TestUpdateWithoutId()
            {
                var actionToUpdate = new WebHookAction
                {
                    Name = "Updated Action",
                    RequestUrl = "test.me",
                    Client = new Client
                    {
                        ApiKey = "xxx",
                        Id = 2,
                        Name = "Client 3"
                    }
                };

                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.UpdateAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await Assert.ThrowsAsync<ArgumentException>(async () => await manager.UpdateActionAsync(actionToUpdate));
                actionDaoMock.Verify(x => x.UpdateAsync(It.IsAny<BaseAction>()), Times.Never());
            }

        }


        public class DeleteTests
        {
            [Fact]
            public async Task TestDelete()
            {
                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.DeleteAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await manager.DeleteActionAsync(actions[0]);
                actionDaoMock.Verify(x => x.DeleteAsync(It.IsAny<BaseAction>()), Times.Once());
            }
            [Fact]
            public async Task TestDeleteWithNull()
            {
                var actionDaoMock = GetDefaultActionDaoMock();
                actionDaoMock.Setup(x => x.DeleteAsync(It.IsAny<BaseAction>())).Verifiable();
                var manager = new ActionManager(actionDaoMock.Object, null);
                await manager.DeleteActionAsync(null);
                actionDaoMock.Verify(x => x.DeleteAsync(It.IsAny<BaseAction>()), Times.Never());
            }
        }
    }
}
