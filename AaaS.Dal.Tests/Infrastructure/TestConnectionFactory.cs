using AaaS.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Tests.Infrastructure
{
    public class TestConnectionFactory : IConnectionFactory
    {
        private DbProviderFactory dbProviderFactory;

        public string ConnectionString => "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Temp\\AaaSTestDb.mdf;Integrated Security=True;MultipleActiveResultSets=False;Connect Timeout=30";

        public string ProviderName => "Microsoft.Data.SqlClient";

        public TestConnectionFactory()
        {
            DbUtil.RegisterAdoProviders();
            this.dbProviderFactory = DbProviderFactories.GetFactory(ProviderName);
        }
        public async Task<DbConnection> CreateConnectionAsync()
        {
            DbConnection connection = dbProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();         
            return connection;
        }
    }
}
