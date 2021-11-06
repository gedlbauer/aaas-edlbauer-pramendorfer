using System.Data.Common;
using System.Threading.Tasks;

namespace AaaS.Common
{
    public interface IConnectionFactory
    {
        string ConnectionString { get; }
        string ProviderName { get; }
        Task<DbConnection> CreateConnectionAsync();
    }
}