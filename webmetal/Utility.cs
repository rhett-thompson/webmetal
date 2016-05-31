using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace webmetal
{
    public class Utility
    {

        public static object ChangeType(object value, Type conversionType)
        {

            if (value == null)
                if (conversionType.IsValueType)
                    return Activator.CreateInstance(conversionType);
                else
                    return null;

            if (value.GetType() == conversionType)
                return value;

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                conversionType = Nullable.GetUnderlyingType(conversionType);

            try
            {
                return TypeDescriptor.GetConverter(conversionType).ConvertFrom(value);
            }
            catch
            {
                return Convert.ChangeType(value, conversionType);

            }

        }

        public static object MapCollectionToObject(NameValueCollection collection, Type type)
        {
            object obj = Activator.CreateInstance(type);
            PropertyInfo[] props = type.GetProperties();


            foreach (PropertyInfo prop in props)
            {

                if (collection[prop.Name] == null)
                    continue;
                
                object value = ChangeType(collection[prop.Name], prop.PropertyType);
                prop.SetValue(obj, value, null);

            }

            return obj;

        }

    }
}