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
using System.Transactions;

namespace AaaS.Dal.Ado
{
    public abstract class AdoActionDao<T> : IActionDao<T> where T : AaaSAction
    {
        private readonly AdoTemplate template;
        private readonly IObjectPropertyDao objectPropertyDao;
        protected abstract string LastInsertedIdQuery { get; }

        public AdoActionDao(IConnectionFactory factory, IObjectPropertyDao objectPropertyDao)
        {
            template = new AdoTemplate(factory);
            this.objectPropertyDao = objectPropertyDao;
        }

        public IAsyncEnumerable<T> FindAllAsync()
        {
            return template.QueryAsync(
                "SELECT * FROM Action a " +
                "JOIN Object o ON (o.id = a.object_id)", MapRowToAction);
        }

        public async Task<T> FindByIdAsync(int id)
        {
            var action = await template.QuerySingleAsync(
                "SELECT * FROM Action a " +
                "JOIN Object o ON (o.id = a.object_id) " +
                "WHERE o.id = @id "
                , MapRowToAction,
                new QueryParameter("@id", id));
            if (action is not null)
                await LoadProperties(action);
            return action;
        }

        public async Task InsertAsync(T action)
        {
            const string SQL_INSERT_OBJECT = "INSERT INTO Object (type) values (@type);";
            const string SQL_INSERT_ACTION = "INSERT INTO Action (object_id) values (@object_id);";
            action.Id = Convert.ToInt32(
                await template.ExecuteScalarAsync<object>($"{SQL_INSERT_OBJECT};{LastInsertedIdQuery}",
                new QueryParameter("@type", action.GetType().AssemblyQualifiedName)));
            await template.ExecuteScalarAsync<object>($"{SQL_INSERT_ACTION}; {LastInsertedIdQuery}",
                new QueryParameter("@object_id", action.Id));
            await InsertProperties(action);
        }

        private async Task InsertProperties(T action)
        {
            var properties = action.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propName = property.Name;
                var propTypeName = property.PropertyType.AssemblyQualifiedName;
                var propValue = property.GetValue(action);
                var objectProperty = new ObjectProperty { ObjectId = action.Id, Name = propName, TypeName = propTypeName, Value = JsonSerializer.Serialize(propValue) };
                await objectPropertyDao.InsertAsync(objectProperty);
            }
        }

        private async Task LoadProperties(T action)
        {
            await ObjectLoaderUtilities.LoadPropertiesFromId(action.Id, action, objectPropertyDao);
        }

        public async Task<bool> UpdateAsync(T action)
        {
            return await UpdateProperties(action);
        }

        private async Task<bool> UpdateProperties(T action)
        {
            if(await FindByIdAsync(action.Id) is null)
            {
                return false;
            }
            return await ObjectLoaderUtilities.UpdateProperties(action.Id, action, objectPropertyDao);
        }

        public async Task<T> MapRowToAction(IDataRecord record)
        {
            string typeName = (string)record["type"];
            var type = Type.GetType(typeName);
            var action = (T)Activator.CreateInstance(type);
            action.Id = (int)record["id"];
            if (action is not null)
                await LoadProperties(action);
            return action;
        }

        public async Task<bool> DeleteAsync(T action)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            const string SQL_DELETE_ACTION = "DELETE FROM Action WHERE object_id=@object_id";
            const string SQL_DELETE_OBJECT = "DELETE FROM Object WHERE id=@id";
            int deletedRows = await template.ExecuteAsync(SQL_DELETE_ACTION, new QueryParameter("@object_id", action.Id));
            if (deletedRows > 0)
            {
                await template.ExecuteAsync(SQL_DELETE_OBJECT, new QueryParameter("@id", action.Id));
                scope.Complete();
                return true;
            }
            return false;
        }
    }
}
