#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:   2009-11-21
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 2009-11-21		zhli Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Text;

namespace Kiss.Utils
{
    /// <summary>
    /// Provider some method to handle exception.
    /// </summary>
    public static class ExceptionUtil
    {
        /// <summary>
        /// Write the exception, include it's inner exception if exists.
        /// </summary>
        public static string WriteException(Exception ex, bool ignoreKissException)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Exception throws at {0}\n", DateTime.Now);

            while (ex != null)
            {
                if (!(ignoreKissException && ex is KissException))
                {
                    sb.Append("Type      :");
                    sb.AppendLine(ex.GetType().FullName);
                    sb.Append("Message   :");
                    sb.AppendLine(ex.Message);
                    sb.Append("Source    :");
                    sb.AppendLine(ex.Source);
                    sb.Append("StackTrace:");
                    sb.AppendLine(ex.StackTrace.Trim());
                }

                ex = ex.InnerException;
            }

            return sb.ToString();
        }

        public static string WriteException(Exception ex)
        {
            return WriteException(ex, false);
        }
    }
}
