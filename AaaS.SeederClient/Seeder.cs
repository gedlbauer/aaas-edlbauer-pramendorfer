using AaaS.Common;
using AaaS.Core.Actions;
using AaaS.Core.Detectors;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.SeederClient
{
    public class Seeder
    {
        private string basePath;
        private IConnectionFactory connectionFactory;
        private IActionDao<BaseAction> actionDao;
        private IClientDao clientDao;
        private IDetectorDao<BaseDetector, BaseAction> detectorDao;

        public Seeder(IConnectionFactory connectionFactory, IActionDao<BaseAction> actionDao, IClientDao clientDao, IDetectorDao<BaseDetector, BaseAction> detectorDao, string basePath)
        {
            this.basePath = basePath;
            this.actionDao = actionDao;
            this.clientDao = clientDao;
            this.detectorDao = detectorDao;
            this.connectionFactory = connectionFactory;
        }

        public async Task DestroyDatabase()
        {
            await ExecuteScriptFromFile(@"DbScripts\drop_database.sql");
        }

        public async Task CreateDatabase()
        {
            await ExecuteScriptFromFile(@"DbScripts\create_database.ddl");
        }

        public async Task RecreateDatabase()
        {
            await DestroyDatabase();
            await CreateDatabase();
        }

        public async Task SeedAll()
        {
            await SeedClients();
            await SeedLogTypes();
            await SeedTelemetries();
            await SeedActions();
            await SeedDetectors();
        }

        public async Task SeedClients()
        {
            await BulkInsert("clients.csv", "Client");
        }

        public async Task SeedLogTypes()
        {
            await BulkInsert("logtypes.csv", "LogType");
        }

        public async Task SeedTelemetries()
        {
            await BulkInsert("telemetry1.csv", "Telemetry");
            await BulkInsert("telemetry2.csv", "Telemetry");
            await BulkInsert("telemetry3.csv", "Telemetry");
            await BulkInsert("logs.csv", "Log");
            await BulkInsert("metrics.csv", "Metric");
            await BulkInsert("timemeasurements.csv", "TimeMeasurement");
        }

        public async Task SeedActions()
        {
            var clients = await clientDao.FindAllAsync().ToListAsync();
            var actions = new List<BaseAction> {
                new MailAction { Client=clients[0], Name="Action 1", MailAddress = "s2010307089@students.fh-hagenberg.at", MailContent = "Hallo! Das ist eine Testmail von Action 1" },
                new MailAction { Client=clients[1], Name="Action 2", MailAddress = "s2010307058@students.fh-hagenberg.at", MailContent = "Hallo! Das ist eine Testmail von Action 2" },
                new MailAction { Client=clients[2], Name="Action 3", MailAddress = "s2010307089@students.fh-hagenberg.at", MailContent = "Hallo! Das ist eine Testmail von Action 3" },
                new MailAction { Client=clients[3], Name="Action 4", MailAddress = "s2010307058@students.fh-hagenberg.at", MailContent = "Hallo! Das ist eine Testmail von Action 4" },
                new MailAction { Client=clients[4], Name="Action 5", MailAddress = "s2010307058@students.fh-hagenberg.at", MailContent = "Hallo! Das ist eine Testmail von Action 5" },

                new WebHookAction { Client=clients[0], Name="Action 6", RequestUrl = "https://www.google.com/" },
                new WebHookAction { Client=clients[1], Name="Action 7", RequestUrl = "https://www.bing.com/" },
                new WebHookAction { Client=clients[2], Name="Action 8", RequestUrl = "https://www.yahoo.com/" },
                new WebHookAction { Client=clients[3], Name="Action 9", RequestUrl = "https://www.youtube.com/" },
                new WebHookAction { Client=clients[4], Name="Action 10", RequestUrl = "https://www.apple.com/" }
            };
            foreach (var action in actions)
            {
                await actionDao.InsertAsync(action);
            }
        }

        public async Task SeedDetectors()
        {
            var actions = await actionDao.FindAllAsync().ToListAsync();
            var clients = await clientDao.FindAllAsync().ToListAsync();
            var detectors = new List<BaseDetector>
            {
                new MinMaxDetector {Client = clients[0], CheckInterval = TimeSpan.FromMilliseconds(1000), Max = 10, Min=1, MaxOccurs = 2, TelemetryName="TestTelemetry1", TimeWindow=TimeSpan.FromSeconds(10)},
                new MinMaxDetector {Client = clients[1], CheckInterval = TimeSpan.FromMilliseconds(1000), Max = 100, Min=10, MaxOccurs = 5, TelemetryName="TestTelemetry2", TimeWindow=TimeSpan.FromSeconds(5)},
                new MinMaxDetector {Client = clients[2], CheckInterval = TimeSpan.FromMilliseconds(1000), Max = 20, Min=2, MaxOccurs = 3, TelemetryName="TestTelemetry3", TimeWindow=TimeSpan.FromSeconds(100)},

                new CurrentValueSlidingWindowDetector { Client = clients[3], CheckInterval = TimeSpan.FromMilliseconds(300), Threshold = 20, TimeWindow = TimeSpan.FromSeconds(20), TelemetryName = "TestTelemetry4", UseGreater = true},
                new CurrentValueSlidingWindowDetector { Client = clients[4], CheckInterval = TimeSpan.FromMilliseconds(500), Threshold = 30, TimeWindow = TimeSpan.FromSeconds(50), TelemetryName = "TestTelemetry5", UseGreater = true},
                new CurrentValueSlidingWindowDetector { Client = clients[0], CheckInterval = TimeSpan.FromMilliseconds(2000), Threshold = 60, TimeWindow = TimeSpan.FromSeconds(10), TelemetryName = "TestTelemetry1", UseGreater = false},

                new AverageSlidingWindowDetector { Client = clients[1], CheckInterval = TimeSpan.FromMilliseconds(200), Threshold = 90, TimeWindow = TimeSpan.FromSeconds(30), TelemetryName = "TestTelemetry2", UseGreater = false},
                new AverageSlidingWindowDetector { Client = clients[2], CheckInterval = TimeSpan.FromMilliseconds(100), Threshold = 10, TimeWindow = TimeSpan.FromSeconds(50), TelemetryName = "TestTelemetry3", UseGreater = true},

                new SumSlidingWindowDetector { Client = clients[3], CheckInterval = TimeSpan.FromMilliseconds(400), Threshold = 100, TimeWindow = TimeSpan.FromSeconds(30), TelemetryName = "TestTelemetry4", UseGreater = true},
                new SumSlidingWindowDetector { Client = clients[4], CheckInterval = TimeSpan.FromMilliseconds(700), Threshold = 200, TimeWindow = TimeSpan.FromSeconds(50), TelemetryName = "TestTelemetry5", UseGreater = false}
            };

            for (int i = 0; i < actions.Count; i++)
            {
                detectors[i].Action = (BaseAction)actions[i];
            }

            foreach (var detector in detectors)
            {
                await detectorDao.InsertAsync(detector);
            }
        }

        private async Task BulkInsert(string fileName, string tableName)
        {
            var sql = $"BULK INSERT {tableName} FROM " +
                $"'{basePath}{fileName}' " +
                $"WITH(FIRSTROW = 2, FIELDTERMINATOR = ',', ROWTERMINATOR = '0x0a'); ";
            await ExecuteScript(sql);
        }

        private async Task ExecuteScriptFromFile(string filepath)
        {
            string seedScript = await File.ReadAllTextAsync(filepath);
            await ExecuteScript(seedScript);
        }

        private async Task ExecuteScript(string script)
        {
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using DbCommand command = connection.CreateCommand();
            command.CommandText = script;
            command.ExecuteNonQuery();
        }
    }
}
