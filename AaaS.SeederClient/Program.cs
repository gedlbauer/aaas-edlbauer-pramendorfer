using AaaS.Common;
using AaaS.Dal.Ado;
using AaaS.Dal.Ado.Telemetry;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AaaS.SeederClient
{
    class Program
    {
        private readonly string BASE_PATH = @"C:\temp\AaaS\seeding\";
        static async Task Main(string[] args)
        {
            IConfiguration config = ConfigurationUtil.GetConfiguration();
            IConnectionFactory factory = DefaultConnectionFactory.FromConfiguration(config, "AaaSDbConnection");

            await new SampleClient(new MSSQLClientDao(factory), new MSSQLLogDao(factory), new MSSQLMetricDao(factory), new MSSQLTimeMeasurementDao(factory)).PrintStats();
        }
    }
}
