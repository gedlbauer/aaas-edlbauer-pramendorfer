using AaaS.Core;
using AaaS.Dal.Ado;
using AaaS.Dal.Interface;
using AaaS.Domain;
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
        private readonly ITestOutputHelper output;

        public DetectorTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            detectorDao = new MSSQLDetectorDao(this.fixture.ConnectionFactory);
            this.output = output;
        }

        [Fact]
        public async Task TestFindByIdAsync()
        {
            Detector detector = await detectorDao.FindByIdAsync(2);
            output.WriteLine(detector?.ToString() ?? "<null>");
        }
        
        [Fact]
        public async Task TestFindAllAsync()
        {
            var sd = new SimpleDetector();
            var detectors = detectorDao.FindAllAsync();
            await foreach(var detector in detectors)
            {
                output.WriteLine(detector?.ToString() ?? "<null>");
            }
        }
    }
}
