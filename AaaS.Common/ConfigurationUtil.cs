using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Common
{
    public static class ConfigurationUtil
    {
        private static IConfiguration configuration = null;

        public static IConfiguration GetConfiguration() =>
          // In C# 8.0 the new ??= operator can be used here
          configuration ??= new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        public static (string connectionString, string providerName) GetConnectionParameters(string configName)
        {
            var connectionConfig = GetConfiguration().GetSection("ConnectionStrings").GetSection(configName);
            return (connectionConfig["ConnectionString"], connectionConfig["ProviderName"]);
        }
    }
}
