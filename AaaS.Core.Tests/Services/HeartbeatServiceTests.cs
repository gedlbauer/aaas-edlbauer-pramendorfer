using Moq;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using AaaS.Core.Options;
using AaaS.Core.HostedServices;
using System.Threading;
using SendGrid.Helpers.Mail;

namespace AaaS.Core.Tests.Services
{
    public class HeartbeatServiceTests
    {
        
        private static IOptions<HeartbeatOptions> Options => Microsoft.Extensions.Options.Options.Create(new HeartbeatOptions()
        {
            Interval = 100,
            Threshold = 100,
            WarningEMailAddress = "testmail@google.com"
        });

        [Fact]
        public async Task TestSuccessfullLogoff()
        {
            var sendGridClientMock = new Mock<ISendGridClient>();
   
            var hearbeatService = new HeartbeatService(sendGridClientMock.Object, Options);
            var tokenSource = new CancellationTokenSource();
            var creator = Guid.NewGuid();

            await hearbeatService.StartAsync(tokenSource.Token);
            hearbeatService.AddHeartbeat(creator);
            hearbeatService.LogOffClient(creator);
            await Task.Delay(400);

            sendGridClientMock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Never);
            tokenSource.Cancel();
        }

        [Fact]
        public async Task TestAnomalyDetection()
        {
            var sendGridClientMock = new Mock<ISendGridClient>();
            var hearbeatService = new HeartbeatService(sendGridClientMock.Object, Options);
            var tokenSource = new CancellationTokenSource();
            var creator = Guid.NewGuid();

            await hearbeatService.StartAsync(tokenSource.Token);
            hearbeatService.AddHeartbeat(creator);
            await Task.Delay(400);

            sendGridClientMock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
            tokenSource.Cancel();
        }
    }
}
