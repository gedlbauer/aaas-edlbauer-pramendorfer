using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public abstract class AdoDetectorDao : IDetectorDao
    {
        protected abstract string LastInsertedIdQuery { get; }
        public async IAsyncEnumerable<Detector> FindAllAsync()
        {
            yield break;
        }

        public async Task<Detector> FindByIdAsync(int id)
        {
            return await Task.FromResult<Detector>(null);
        }

        public static Detector MapRowToDetector(IDataRecord record)
        {
            string typeName = (string)record["type"];
            var assembly = Assembly.GetExecutingAssembly().FullName;
            var type = Type.GetType(typeName);
            var detector = (Detector)Activator.CreateInstance(type);
            return detector;
        }
    }
}
