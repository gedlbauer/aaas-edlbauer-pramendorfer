using AaaS.Dal.Interface;
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
    }
}
