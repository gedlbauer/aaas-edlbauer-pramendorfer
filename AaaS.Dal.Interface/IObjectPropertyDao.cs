using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Interface
{
    public interface IObjectPropertyDao
    {
        IAsyncEnumerable<ObjectProperty> FindAllAsync();

        Task<ObjectProperty> FindByObjectIdAndNameAsync(int objectId, string name);

        IAsyncEnumerable<ObjectProperty> FindByObjectIdAsync(int objectId);

        Task<bool> UpdateAsync(ObjectProperty obj);

        Task InsertAsync(ObjectProperty obj);

        Task<bool> DeleteAsync(ObjectProperty obj);
    }
}
