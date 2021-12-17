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
        public static Log SampleLog => new()
        {
            Client = new Client
            {
                ApiKey = "lskjfasdf",
                Id = 1,
                Name = "test-user"
            },
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


    }
}
