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
            var detectors = template.QueryAsync(
                "SELECT * FROM Detector d " +
                "JOIN Object o ON (o.id = d.object_id) ",
                MapRowToDetector);
            await foreach (var detector in detectors)
            {
                yield return detector;
            }
        }

        public async Task<Detector> FindByIdAsync(int id)
        {
            return await template.QuerySingleAsync(
                "SELECT * FROM Detector d " +
                "JOIN Object o ON (o.id = d.object_id) " +
                "WHERE o.id = @id "
                , MapRowToDetector,
                new QueryParameter("@id", id)
                );
        }

        public static Detector MapRowToDetector(IDataRecord record)
        {
            string typeName = (string)record["type"];
            var type = Type.GetType(typeName);
            var detector = (Detector) Activator.CreateInstance(type);
            return detector;
        }
    }
}
