using AaaS.Common;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AaaS.Dal.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public IConnectionFactory ConnectionFactory { get; private set; }

        public DatabaseFixture()
        {
            ConnectionFactory = new TestConnectionFactory();
        }

        public void Dispose() { }

    }
}
