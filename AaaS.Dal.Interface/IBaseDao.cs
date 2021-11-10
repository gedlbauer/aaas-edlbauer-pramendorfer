using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Interface
{
    public interface IBaseDao<T>
    {
        IAsyncEnumerable<T> FindAllAsync();

        Task<T> FindByIdAsync(int id);

        Task<bool> UpdateAsync(T obj);

        Task InsertAsync(T obj);

        Task<bool> DeleteAsync(T obj);
    }
}
