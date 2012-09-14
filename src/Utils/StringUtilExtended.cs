using System;
using System.Collections.Generic;

namespace Kiss.Utils
{
    /// <summary>
    /// Miscellaneous <see cref="System.String"/> utility methods.
    /// </summary>
    public static class StringUtilExtended
    {
        #region Extension method

        public static int ToInt(this string str) { return StringUtil.ToInt(str); }
        public static int ToInt(this string str, int defaultValue) { return StringUtil.ToInt(str, defaultValue); }
        public static decimal ToDecimal(this string str) { return StringUtil.ToDecimal(str); }
        public static decimal ToDecimal(this string str, decimal defaultValue) { return StringUtil.ToDecimal(str, defaultValue); }
        public static bool ToBoolean(this string str) { return StringUtil.ToBoolean(str, false); }
        public static bool ToBoolean(this string str, bool defaultValue) { return StringUtil.ToBoolean(str, defaultValue); }
        public static DateTime ToDateTime(this string str) { return StringUtil.ToDateTime(str); }
        public static DateTime ToDateTime(this string str, DateTime defaultValue) { return StringUtil.ToDateTime(str, defaultValue); }

        public static string Join(this List<string> str)
        {
            return StringUtil.CollectionToCommaDelimitedString(str);
        }

        public static string Join(this List<string> str, string separtor)
        {
            return StringUtil.CollectionToDelimitedString(str, separtor, string.Empty);
        }

        public static string Join(this List<string> str, string separtor, string surround)
        {
            return StringUtil.CollectionToDelimitedString(str, separtor, surround);
        }

        #endregion
    }
}
