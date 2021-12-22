using AaaS.Core.Actions;
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
    public class SlidingWindowDetectorTests
    {
        private class TestDetector : SlidingWindowDetector
        {
            public TestDetector() : base(null)
            {
            }
            protected override Task<double> CalculateCheckValue()
            {
                return Task.FromResult(3d);
            }
        }

        [Fact]
        public async Task TestActionExecuteFiresOnAnomalyWhenGreaterIsUsed()
        {
            var actionMock = new Mock<BaseAction>();
            actionMock.Setup(action => action.Execute()).Verifiable();
            var det = new TestDetector
            {
                Action = actionMock.Object,
                Threshold = 2,
                UseGreater = true,
                CheckInterval = TimeSpan.FromMilliseconds(200)
            };
            await det.Start();
            await Task.Delay(10);
            det.Stop();
            actionMock.Verify(x => x.Execute(), Times.Once());
        }

        [Fact]
        public async Task TestActionExecuteFiresOnAnomalyWhenSmallerIsUsed()
        {
            var actionMock = new Mock<BaseAction>();
            actionMock.Setup(action => action.Execute()).Verifiable();
            var det = new TestDetector
            {
                Action = actionMock.Object,
                Threshold = 4,
                UseGreater = false,
                CheckInterval = TimeSpan.FromMilliseconds(200)
            };
            await det.Start();
            await Task.Delay(10);
            det.Stop();
            actionMock.Verify(x => x.Execute(), Times.Once());
        }

        [Fact]
        public async Task TestActionExecuteDoesnotFireOnNoAnomalyWhenGreaterIsUsed()
        {
            var actionMock = new Mock<BaseAction>();
            actionMock.Setup(action => action.Execute()).Verifiable();
            var det = new TestDetector
            {
                Action = actionMock.Object,
                Threshold = 3,
                UseGreater = true,
                CheckInterval = TimeSpan.FromMilliseconds(50)
            };
            await det.Start();
            await Task.Delay(10);
            det.Stop();
            actionMock.Verify(x => x.Execute(), Times.Never());
        }

        [Fact]
        public async Task TestActionDoesNotExecuteFireOnNoAnomalyWhenSmallerIsUsed()
        {
            var actionMock = new Mock<BaseAction>();
            actionMock.Setup(action => action.Execute()).Verifiable();
            var det = new TestDetector
            {
                Action = actionMock.Object,
                Threshold = 3,
                UseGreater = false,
                CheckInterval = TimeSpan.FromMilliseconds(50)
            };
            await det.Start();
            await Task.Delay(10);
            det.Stop();
            actionMock.Verify(x => x.Execute(), Times.Never());
        }
    }
}
