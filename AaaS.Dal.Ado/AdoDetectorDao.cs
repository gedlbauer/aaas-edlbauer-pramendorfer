using AaaS.Common;
using AaaS.Dal.Ado.Utilities;
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
        private readonly AdoTemplate template;

        public AdoDetectorDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }

        protected abstract string LastInsertedIdQuery { get; }
        public async IAsyncEnumerable<Detector> FindAllAsync()
        {
            yield break;
        }

        public async Task<Detector> FindByIdAsync(int id)
        {
            return await template.QuerySingleAsync(
                "SELECT * FROM Object o" +
                "WHERE id = @id" +
                "JOIN Detector d ON (o.id = d.object_id)"
                , MapRowToDetector,
                new QueryParameter("@id", id)
                );
        }

        public static Detector MapRowToDetector(IDataRecord record)
        {
            string typeName = (string)record["type"];
            var assembly = Assembly.GetCallingAssembly();
            var detector = (Detector)assembly.CreateInstance(typeName);
            return detector;
        }
    }
}
