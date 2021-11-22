using AaaS.Common;
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

        public Seeder(IConnectionFactory connectionFactory, string basePath)
        {
            this.basePath = basePath;
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
