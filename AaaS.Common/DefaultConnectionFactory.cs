using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AaaS.Common
{
    public class DefaultConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory dbProviderFactory;

        public static IConnectionFactory FromConfiguration(IConfiguration config, string connectionStringConfigName)
        {
            var connectionConfig = config.GetSection("ConnectionStrings").GetSection(connectionStringConfigName);
            string connectionString = connectionConfig["ConnectionString"];
            string providerName = connectionConfig["ProviderName"];

            return new DefaultConnectionFactory(connectionString, providerName);
        }

        public DefaultConnectionFactory(string connectionString, string providerName)
        {
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;

            DbUtil.RegisterAdoProviders();
            this.dbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public string ConnectionString { get; }

        public string ProviderName { get; }

        public async Task<DbConnection> CreateConnectionAsync()
        {
            DbConnection connection = dbProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            await connection.OpenAsync();
            return connection;
        }
    }
}
