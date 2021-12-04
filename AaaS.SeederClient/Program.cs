using AaaS.Common;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
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
        private static readonly string BASE_PATH = @"C:\temp\AaaS\seeding\";
        static async Task Main(string[] args)
        {
            IConfiguration config = ConfigurationUtil.GetConfiguration();
            IConnectionFactory factory = DefaultConnectionFactory.FromConfiguration(config, "AaaSDbConnection");
            var seeder = new Seeder(factory, new MSSQLActionDao<BaseAction>(factory), new MSSQLClientDao(factory), new MSSQLDetectorDao<BaseDetector, BaseAction>(factory), BASE_PATH);

            await seeder.RecreateDatabase();
            await seeder.SeedAll();

            await new SampleClient(new MSSQLClientDao(factory), new MSSQLLogDao(factory), new MSSQLMetricDao(factory), new MSSQLTimeMeasurementDao(factory)).PrintStats();
        }
    }
}
