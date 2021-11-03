using System.Data.Common;
using System.Threading.Tasks;

namespace AaaS.Core.Dal.Utils
{
    public interface IConnectionFactory
    {
        string ConnectionString { get; }
        string ProviderName { get; }
        Task<DbConnection> CreateConnectionAsync();
    }
}