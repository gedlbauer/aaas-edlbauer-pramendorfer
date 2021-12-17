using AaaS.Core.Detectors;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AaaS.Core.Tests.Detectors
{
    public class BaseDetectorTests
    {
        [Fact]
        public async Task CheckIntervalIsRight()
        {
            var detectorMock = new Mock<BaseDetector>(null);
            detectorMock.Protected().Setup("Detect").Verifiable();
            detectorMock.Object.CheckInterval = TimeSpan.FromMilliseconds(100);
            await detectorMock.Object.Start();
            await Task.Delay(180);
            detectorMock.Object.Stop();
            detectorMock.Protected().Verify("Detect", Times.Exactly(2));
        }
    }
}
