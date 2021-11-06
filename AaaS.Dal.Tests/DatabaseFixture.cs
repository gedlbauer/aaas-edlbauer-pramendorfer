using AaaS.Common;
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
        private TransactionScope _transactionScope;
        public DatabaseFixture()
        {
            _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            ConnectionFactory = new TestConnectionFactory();
           
        }

        public void Dispose()
        {
            _transactionScope.Dispose();
            // ... clean up test data from the database ...
        }

    }
}
