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
using System.Text.Json;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public abstract class AdoObjectPropertyDao : IObjectPropertyDao
    {
        private readonly AdoTemplate template;
        protected abstract string LastInsertedIdQuery { get; }

        public AdoObjectPropertyDao(IConnectionFactory connectionFactory)
        {
            template = new AdoTemplate(connectionFactory);
        }

        public IAsyncEnumerable<ObjectProperty> FindAllAsync()
        {
            const string SQL = "SELECT * FROM ObjectProperty";
            return template.QueryAsync(SQL, MapRowToObjectProperty);
        }

        private ObjectProperty MapRowToObjectProperty(IDataRecord record)
        {
            return new ObjectProperty
            {
                Name = (string)record["name"],
                Value = (string)record["value"],
                TypeName = (string)record["type"],
                ObjectId = (int)record["object_id"]
            };
        }

        public async Task<ObjectProperty> FindByObjectIdAndNameAsync(int objectId, string name)
        {
            const string SQL = "SELECT * FROM ObjectProperty WHERE object_id=@object_id AND name=@name";
            return await template.QuerySingleAsync(
                SQL,
                MapRowToObjectProperty,
                new QueryParameter("@object_id", objectId),
                new QueryParameter("@name", name));
        }

        public IAsyncEnumerable<ObjectProperty> FindByObjectIdAsync(int objectId)
        {
            const string SQL = "SELECT * FROM ObjectProperty WHERE object_id=@object_id";
            return template.QueryAsync(SQL, MapRowToObjectProperty, new QueryParameter("@object_id", objectId));
        }

        public async Task<bool> UpdateAsync(ObjectProperty prop)
        {
            const string SQL =
                "UPDATE ObjectProperty " +
                "SET value=@value, type=@type " +
                "WHERE object_id=@object_id AND name=@name";
            return (await template.ExecuteAsync(SQL,
                new QueryParameter("@object_id", prop.ObjectId),
                new QueryParameter("@name", prop.Name),
                new QueryParameter("@type", prop.TypeName),
                new QueryParameter("@value", prop.Value))) > 0;
        }

        public async Task InsertAsync(ObjectProperty prop)
        {
            const string SQL_INSERT = "INSERT INTO ObjectProperty (object_id, name, type, value) values (@object_id, @name, @type, @value);";
            await template.ExecuteScalarAsync<object>($"{SQL_INSERT};{LastInsertedIdQuery}",
             new QueryParameter("@object_id", prop.ObjectId),
             new QueryParameter("@name", prop.Name),
             new QueryParameter("@type", prop.TypeName),
             new QueryParameter("@value", prop.Value));
        }

        public async Task<bool> DeleteAsync(ObjectProperty prop)
        {
            const string SQL =
                "DELETE FROM ObjectProperty " +
                "WHERE object_id = @object_id AND name=@name";
            return (await template.ExecuteAsync(
                SQL,
                new QueryParameter("@object_id", prop.ObjectId),
                new QueryParameter("@name", prop.Name))) > 0;
        }
    }
}
