using System.Data;
using System.Globalization;
using System.Reflection;

namespace Avalanche.QueryModel.Data
{
    // https://www.exceptionnotfound.net/mapping-datatables-and-datarows-to-objects-in-csharp-and-net-using-reflection/
    // https://github.com/exceptionnotfound/DataNamesMappingDemo
    public class EntityMapper<TEntity> where TEntity : class, new()
    {
        public TEntity Map(DataRow row)
        {
            var entity = new TEntity();
            return Map(row, entity);
        }

        public TEntity Map(DataRow row, TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties()
                .ToList();
            foreach (var prop in properties)
            {
                PropertyMapper.Map(typeof(TEntity), row, prop, entity);
            }

            return entity;
        }

        public IEnumerable<TEntity> Map(DataTable table)
        {
            var entities = new List<TEntity>();
            var properties = typeof(TEntity).GetProperties()
                .ToList();
            foreach (DataRow row in table.Rows)
            {
                var entity = new TEntity();
                foreach (var prop in properties)
                {
                    PropertyMapper.Map(typeof(TEntity), row, prop, entity);
                }
                entities.Add(entity);
            }

            return entities;
        }
    }

    public static class PropertyMapper
    {
        public static void Map(Type type, DataRow row, PropertyInfo prop, object entity)
        {
            var fieldName = prop.Name;

            var columnName = AttributeHelper.GetDataName(type, prop.Name);
            if (!string.IsNullOrEmpty(columnName))
            {
                fieldName = columnName;
            }

            if (!string.IsNullOrWhiteSpace(fieldName) && row.Table.Columns.Contains(fieldName))
            {
                var propertyValue = row[fieldName];
                if (propertyValue != DBNull.Value)
                {
                    ParsePrimitive(prop, entity, row[fieldName]);
                }
            }
        }

        public static void Map<T>(T entity, object item, PropertyData header)
        {
            if (item == null)
            {
                return;
            }

            ParsePrimitive(header.PropertyInfo, entity, item);
        }

        public static void ParsePrimitive(PropertyInfo prop, object entity, object value)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(entity, value.ToString().Trim(), null);
            }
            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, ParseBoolean(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(long))
            {
                prop.SetValue(entity, long.Parse(value.ToString()), null);
            }
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {
                if (value == null)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, int.Parse(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(decimal))
            {
                prop.SetValue(entity, decimal.Parse(value.ToString()), null);
            }
            else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
            {
                var isValid = double.TryParse(value.ToString(), out _);
                if (isValid)
                {
                    prop.SetValue(entity, double.Parse(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                var isValid = DateTime.TryParse(value.ToString(), out var date);
                if (isValid)
                {
                    prop.SetValue(entity, date, null);
                }
                else
                {
                    isValid = DateTime.TryParseExact(value.ToString(), "yyyyMMdd", new CultureInfo("de-CH"), DateTimeStyles.AssumeLocal, out date);
                    if (isValid)
                    {
                        prop.SetValue(entity, date, null);
                    }
                }
            }
            else if (prop.PropertyType == typeof(Guid))
            {
                var isValid = Guid.TryParse(value.ToString(), out var guid);
                if (isValid)
                {
                    prop.SetValue(entity, guid, null);
                }
                else
                {
                    isValid = Guid.TryParseExact(value.ToString(), "B", out guid);
                    if (isValid)
                    {
                        prop.SetValue(entity, guid, null);
                    }
                }


            }
        }

        public static bool ParseBoolean(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            switch (value.ToString().ToLowerInvariant())
            {
                case "1":
                case "y":
                case "yes":
                case "true":
                    return true;

                case "0":
                case "n":
                case "no":
                case "false":
                default:
                    return false;
            }
        }
    }

    public class PropertyData
    {
        public string Name { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string BindingName { get; set; }
    }



    public static class AttributeHelper
    {
        public static string GetDataName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName)?.GetCustomAttribute<DataNameAttribute>();
            if (property != null)
            {
                return property.ValueName;
            }

            return null;
        }

        public static IEnumerable<PropertyData> GetPropertyData(this Type type)
        {
            var data = new List<PropertyData>();
            foreach (var prop in type.GetProperties())
            {
                var itm = new PropertyData
                {
                    Name = prop.Name,
                    BindingName = prop.Name,
                    PropertyInfo = prop
                };

                var attr = prop.GetCustomAttribute<DataNameAttribute>();
                if (attr != null)
                {
                    itm.BindingName = attr.ValueName;
                }

                data.Add(itm);
            }

            return data;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataNameAttribute : Attribute
    {
        public string ValueName { get; set; }

        public DataNameAttribute(string valueName)
        {
            ValueName = valueName;
        }
    }
}
