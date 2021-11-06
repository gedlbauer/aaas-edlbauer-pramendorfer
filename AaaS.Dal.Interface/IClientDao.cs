using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Interface
{
    public interface IClientDao
    {
        IAsyncEnumerable<Client> FindAllAsync();

        Task<Client> FindByIdAsync(int id);

        Task<bool> UpdateAsync(Client client);

        Task InsertAsync(Client client);
    }
}
