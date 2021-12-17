using AaaS.Core.Repositories;
using AaaS.Core.Tests.Infrastructure;
using AaaS.Dal.Interface;
using AaaS.Domain;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Repositories
{
    public class LogRepositoryTests
    {

        private static Client SampleClient = new()
        {
            Id = 1,
            ApiKey = "api-key",
            Name = "test-user"
        };
        private static Log SampleLog => new()
        {
            Client = SampleClient,
            CreatorId = Guid.Parse("2f729e58-3fe4-4dab-964f-912fc0d845fd"),
            Id = 1,
            Message = "test-message",
            Name = "test-log",
            Timestamp = DateTime.Now,
            Type = new LogType
            {
                Id = 1,
                Name = "Error"
            }
        };

        private static IAsyncEnumerable<Log> Logs => new List<Log>
        {
            SampleLog,
            new Log() {
                Client = SampleClient,
                CreatorId = Guid.Parse("2f729e58-3fe4-4dab-964f-912fc0d845fd"),
                Id = 2,
                Message = "test-message 2",
                Name = "test-log",
                Timestamp = DateTime.Now,
                Type = new LogType
                {
                    Id = 1,
                    Name = "Error"
                }
            },
            new Log() {
                Client = new Client{
                    Id=2,
                    ApiKey="api-key2",
                    Name="test-client-2"
                },
                CreatorId = Guid.Parse("2f729e58-3fe4-4dab-964f-912fc0d845fa"),
                Id = 2,
                Message = "test-message 2",
                Name = "test-log",
                Timestamp = DateTime.Now,
                Type = new LogType
                {
                    Id = 1,
                    Name = "Error"
                }
            }
        }.ToAsyncEnumerable();

        private static IAsyncEnumerable<LogType> LogTypes => new List<LogType>
        {
            new LogType { Id = 1, Name = "Error" },
            new LogType { Id = 2, Name = "Message" },
            new LogType { Id = 3, Name = "Warning" }
        }.ToAsyncEnumerable();

        [Fact]
        public async Task TestInsert()
        {
            var logDaoMock = new Mock<ILogDao>();
            var logRepo = new LogRepository(logDaoMock.Object);
            await logRepo.InsertAsync(SampleLog);

            logDaoMock.Verify(x => x.InsertAsync(It.IsAny<Log>()), Times.Once());
        }

        [Fact]
        public async Task TestFindById()
        {
            var logDaoMock = new Mock<ILogDao>();
            logDaoMock.Setup(x => x.FindByIdAndClientAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(SampleLog));

            var logRepo = new LogRepository(logDaoMock.Object);
            var actualLog = await logRepo.FindByIdAsync(SampleLog.Id, SampleLog.Client.Id);

            actualLog.Should().BeEquivalentTo(SampleLog, FluentAssertionsExtensions.ApproximateDateTime);
        }

        [Fact]
        public async Task TestFindByNonExistingIdReturnsNull()
        {
            var logDaoMock = new Mock<ILogDao>();
            var logRepo = new LogRepository(logDaoMock.Object);
            (await logRepo.FindByIdAsync(SampleLog.Id, SampleLog.Client.Id)).Should().BeNull();
        }

        [Fact]
        public async Task TestFindAllLogTypes()
        {
            var logDaoMock = new Mock<ILogDao>();
            var logRepo = new LogRepository(logDaoMock.Object);
            logDaoMock.Setup(x => x.FindAllLogTypesAsync()).Returns(LogTypes);

            (await logRepo.FindAllLogTypesAsync().ToListAsync())
                .Should()
                .BeEquivalentTo(await LogTypes.ToListAsync(), o => o.WithoutStrictOrdering());
        }

        [Fact]
        public async Task TestFindAll()
        {
            var logDaoMock = new Mock<ILogDao>();
            var logRepo = new LogRepository(logDaoMock.Object);
            logDaoMock.Setup(x => x
                .FindAllByClientAsync(It.IsAny<int>()))
                .Returns<int>(i => Logs.Where(l => l.Client.Id ==i));

            (await logRepo.FindAllAsync(1).ToListAsync())
                .Should()
                .BeEquivalentTo(
                    await Logs.Where(x => x.Client.Id == 1).ToListAsync(),
                    FluentAssertionsExtensions.ApproximateDateTime);
        }
        
        [Fact]
        public async Task TestFindByName()
        {
            var logDaoMock = new Mock<ILogDao>();
            var logRepo = new LogRepository(logDaoMock.Object);
            logDaoMock.Setup(x => x.FindAllByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .Returns<int, string>((i, s) => Logs.Where(l => l.Client.Id == i && l.Name == s));

            (await logRepo.FindByAllByNameAsync(2, "test-log").ToListAsync())
                .Should()
                .OnlyContain(x => x.Client.Id == 2 && x.Name == "test-log");
        }

        [Fact]
        public async Task TestFindByCreator()
        {
            var logDaoMock = new Mock<ILogDao>();
            var logRepo = new LogRepository(logDaoMock.Object);
            logDaoMock.Setup(x => x.FindByCreatorAsync(It.IsAny<int>(), It.IsAny<Guid>()))
                .Returns(Logs);

            (await logRepo.FindByCreatorAsync(1, Guid.NewGuid()).ToListAsync())
                .Should()
                .BeEquivalentTo(await Logs.ToListAsync(), FluentAssertionsExtensions.ApproximateDateTime);
        }
    }
}
