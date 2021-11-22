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
            await ExecuteScript(@"DbScripts\drop_database.sql");
        }

        public async Task CreateDatabase()
        {
            await ExecuteScript(@"DbScripts\create_datbase.ddl");
        }

        public async Task RecreateDatabase()
        {
            await DestroyDatabase();
            await CreateDatabase();
        }

        private async Task ExecuteScript(string filepath)
        {
            string seedScript = await File.ReadAllTextAsync(filepath);
            await using DbConnection connection = await connectionFactory.CreateConnectionAsync();
            await using DbCommand command = connection.CreateCommand();
            command.CommandText = seedScript;
            command.ExecuteNonQuery();
        }
    }
}
