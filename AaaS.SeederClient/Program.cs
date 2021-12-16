using AaaS.ClientSDK;
using AaaS.ClientSDK.Options;
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

            Console.WriteLine("Starting Recreate...");
            await seeder.RecreateDatabase();
            Console.WriteLine("Starting Seed...");
            await seeder.SeedAll();

            await new SampleClient(new MSSQLClientDao(factory), new MSSQLLogDao(factory), new MSSQLMetricDao(factory), new MSSQLTimeMeasurementDao(factory)).PrintStats();
            //await TestClientSDK();
        }

        //private static async Task TestClientSDK()
        //{
        //    AaaSService aaas = new AaaSService(new System.Net.Http.HttpClient(), new AaaSServiceOptions { ApiKey = "d1447938-c1bd-453d-a2a8-bb1b895fbe65" });

        //    await aaas.InsertLog(new LogInsertDto()
        //    {
        //        CreatorId = Guid.Parse("f6f3ae05-46a0-4f1e-b095-75cbe0bed8d6"),
        //        LogTypeId = 1,
        //        Message = "Test Message",
        //        Name = "Log",
        //        Timestamp = DateTime.Now
        //    });

        //}
    }
}
