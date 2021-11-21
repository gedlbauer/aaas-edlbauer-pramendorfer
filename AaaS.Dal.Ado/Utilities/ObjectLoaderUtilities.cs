using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado.Utilities
{
    public static class ObjectLoaderUtilities
    {
        public static async Task LoadPropertiesFromId<T>(int id, T objectToLoad, IObjectPropertyDao objectPropertyDao)
        {
            var properties = await objectPropertyDao.FindByObjectIdAsync(id).ToListAsync();
            var actionType = objectToLoad.GetType();
            foreach (var property in properties)
            {
                var propertyType = Type.GetType(property.TypeName);
                var jsonSerializerType = typeof(JsonSerializer);
                var parameterTypes = new[] { typeof(string), typeof(JsonSerializerOptions) };
                var deserializerMethod = jsonSerializerType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(i => i.Name.Equals("Deserialize", StringComparison.InvariantCulture))
                    .Where(i => i.IsGenericMethod)
                    .Where(i => i.GetParameters().Select(a => a.ParameterType).SequenceEqual(parameterTypes))
                    .Single();
                var deserializer = deserializerMethod.MakeGenericMethod(propertyType);
                var propertyValue = deserializer.Invoke(null, new object[] { property.Value, new JsonSerializerOptions(JsonSerializerDefaults.General) });
                actionType.GetProperty(property.Name).SetValue(objectToLoad, propertyValue);
            }
        }

        public static async Task<bool> UpdateProperties<T>(int id, T action, IObjectPropertyDao objectPropertyDao)
        {
            var updated = false;
            var properties = action.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.GetValue(action) is null)
                {
                    if (await objectPropertyDao.DeleteAsync(new ObjectProperty { ObjectId = id, Name = property.Name }))
                        updated = true;
                }
                else
                {
                    var objectProperty = new ObjectProperty
                    {
                        ObjectId = id,
                        Name = property.Name,
                        TypeName = property.PropertyType.AssemblyQualifiedName,
                        Value = JsonSerializer.Serialize(property.GetValue(action))
                    };
                    if (await objectPropertyDao.FindByObjectIdAndNameAsync(objectProperty.ObjectId, objectProperty.Name) is not null)
                    {
                        if (await objectPropertyDao.UpdateAsync(objectProperty))
                        {
                            updated = true;
                        }
                    }
                    else
                    {
                        await objectPropertyDao.InsertAsync(objectProperty);
                        updated = true;
                    }
                }
            }
            return updated;
        }
    }
}
