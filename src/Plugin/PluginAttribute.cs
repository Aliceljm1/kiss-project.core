using System;
using Kiss.Security;

namespace Kiss.Plugin
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class PluginAttribute : Attribute, IPlugin
    {
        #region IPlugin Members

        public string Name { get; set; }

        public Type Decorates { get; set; }

        public int SortOrder { get; set; }

        public virtual bool IsAuthorized(Principal user)
        {
            return true;
        }

        public virtual bool IsEnabled
        {
            get { return true; }
        }

        #endregion

        #region IComparable<IPlugin> Members

        public int CompareTo(IPlugin other)
        {
            return SortOrder.CompareTo(other.SortOrder);
        }

        #endregion
    }
}
