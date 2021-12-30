using AaaS.Dal.Ado.Attributes;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado.Utilities
{
    public static class ObjectLoaderUtilities
    {
        private static readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.General);

        static ObjectLoaderUtilities()
        {
            jsonOptions.Converters.Add(new JsonTimeSpanConverter());
        }

        private static IEnumerable<PropertyInfo> GetPropertiesToStoreAsObjectProperty<T, BaseType>(T obj)
        {
            var baseProperties = typeof(BaseType).GetProperties();
            var properties = obj.GetType()
                .GetProperties()
                .Where(prop => !baseProperties.Any(baseProp => baseProp.Name == prop.Name && (baseProp.PropertyType == prop.PropertyType || prop.PropertyType.IsSubclassOf(baseProp.PropertyType)))) // Properties from Base Class are already stored in Db Table
                .Where(prop => Attribute.GetCustomAttribute(prop, typeof(VolatileAttribute)) is null) // Ignore Properties that are defined as Volatile
                .Where(prop => prop.CanWrite);
            return properties;
        }

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
                var propertyValue = deserializer.Invoke(null, new object[] { property.Value, jsonOptions });
                actionType.GetProperty(property.Name).SetValue(objectToLoad, propertyValue);
            }
        }

        public static async Task InsertProperties<T, BaseType>(int objectId, T obj, IObjectPropertyDao objectPropertyDao)
        {
            var properties = GetPropertiesToStoreAsObjectProperty<T, BaseType>(obj);
            foreach (var property in properties)
            {
                var propName = property.Name;
                var propTypeName = property.PropertyType.AssemblyQualifiedName;
                var propValue = property.GetValue(obj);
                var objectProperty = new ObjectProperty { ObjectId = objectId, Name = propName, TypeName = propTypeName, Value = JsonSerializer.Serialize(propValue, jsonOptions) };
                await objectPropertyDao.InsertAsync(objectProperty);
            }
        }

        public static async Task<bool> UpdateProperties<T, BaseType>(int id, T obj, IObjectPropertyDao objectPropertyDao)
        {
            var updated = false;
            var properties = GetPropertiesToStoreAsObjectProperty<T, BaseType>(obj);
            foreach (var property in properties)
            {
                if (property.GetValue(obj) is null)
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
                        Value = JsonSerializer.Serialize(property.GetValue(obj), jsonOptions)
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
