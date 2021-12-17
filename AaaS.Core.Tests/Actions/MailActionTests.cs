using AaaS.Core.Actions;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Actions
{
    public class MailActionTests
    {
        [Fact(Skip = "true")]
        public async Task MailIsSentOnExecute()
        {
            var sendGridMock = new Mock<ISendGridClient>();
            sendGridMock.Setup(
                x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>())
            ).Returns(Task.FromResult<Response>(null)).Verifiable();

            var mailAction = new MailAction(sendGridMock.Object)
            {
                MailAddress = "test@test.test",
                MailContent = "Test"
            };
            await mailAction.Execute();
            sendGridMock.Verify(
                client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
        }
    }
}
