#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:   2009-09-07
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 2009-09-07		zhli Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Kiss.Utils
{
    /// <summary>
    /// use this util to convert type
    /// </summary>
    public static class TypeConvertUtil
    {
        /// <summary>
        /// convert object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(object value)
        {
            return (T)ConvertTo(value, typeof(T));
        }

        /// <summary>
        /// convert to
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ConvertTo(object value, Type targetType)
        {
            // check for value = null, thx alex       
            if (value == null)
                return null;

            // do we have a nullable type?
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter nc = new NullableConverter(targetType);
                targetType = nc.UnderlyingType;
            }

            if (targetType.IsEnum) // if enum use parse
                return Enum.Parse(targetType, value.ToString(), false);
            else
            {
                // if we have a custom type converter then use it
                TypeConverter td = TypeDescriptor.GetConverter(targetType);
                if (td.CanConvertFrom(value.GetType()))
                {
                    return td.ConvertFrom(value);
                }
                else // otherwise use the changetype
                    return Convert.ChangeType(value, targetType);
            }
        }

        /// <summary>
        /// convert namevaluecollection to obj
        /// </summary>
        public static void ConvertFrom(object old, NameValueCollection nv, params string[] ignores)
        {
            if (old == null) return;

            List<string> ignore_list = new List<string>();

            foreach (var item in ignores)
            {
                ignore_list.Add(item.ToLower());
            }

            Type t = old.GetType();

            foreach (string key in nv.Keys)
            {
                if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase)) continue;

                if (ignore_list.Contains(key.ToLower()))
                    continue;

                PropertyInfo prop = t.GetProperty(key);
                if (prop == null || !prop.CanWrite) continue;

                prop.SetValue(old,
                    ConvertTo(nv[key], prop.PropertyType),
                    null);
            }
        }
    }
}
