using AaaS.Common;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace AaaS.Dal.Tests.Infrastructure
{
    [CollectionDefinition("SeededDb")]
    public class DatabaseFixture : IDisposable
    {
        public const string CREATE_PATH = @"DbScripts\create_database.ddl";
        public const string SEED_PATH = @"DbScripts\seed_database.sql";
        public const string DROP_PATH = @"DbScripts\drop_database.sql";
        public IConnectionFactory ConnectionFactory { get; private set; }

        public DatabaseFixture()
        {
            ConnectionFactory = new TestConnectionFactory();
            ExecuteScript(DROP_PATH).Wait();
            ExecuteScript(CREATE_PATH).Wait();
            ExecuteScript(SEED_PATH).Wait();
        }

        public void Dispose() {
            
        }

        private async Task ExecuteScript(string filepath)
        {
            string seedScript = await File.ReadAllTextAsync(filepath);
            await using DbConnection connection = await ConnectionFactory.CreateConnectionAsync();
            await using DbCommand command = connection.CreateCommand();
            command.CommandText = seedScript;
            command.ExecuteNonQuery();
        }
    }
}
