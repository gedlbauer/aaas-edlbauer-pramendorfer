using AaaS.Common;
using AaaS.Dal.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit.Sdk;

namespace AaaS.Dal.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AutoRollbackAttribute : BeforeAfterTestAttribute
    {
        private static IConnectionFactory ConnectionFactory = new TestConnectionFactory();

        public override void After(MethodInfo methodUnderTest)
        {
            ExecuteScript(DatabaseFixture.DROP_PATH).Wait();
            ExecuteScript(DatabaseFixture.CREATE_PATH).Wait();
            ExecuteScript(DatabaseFixture.SEED_PATH).Wait();
        }

        public override void Before(MethodInfo methodUnderTest)
        {

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
