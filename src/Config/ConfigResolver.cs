#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:   2009-09-24
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 2009-09-24		zhli Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gala.Config
{
    /// <summary>
    /// use this class to get config
    /// </summary>
    public class ConfigResolver
    {
        private Dictionary<string, Type> types = new Dictionary<string, Type>();

        public ConfigResolver()
        {
            foreach (Assembly asm in ServiceLocator.Instance.Resolve<ITypeFinder>().GetAssemblies())
            {
                if (asm.GetCustomAttributes(typeof(ConfigAttribute), false).Length == 0)
                    continue;

                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(ConfigBase)))
                        types[type.Assembly.FullName] = type;
                }
            }
        }

        public ConfigBase Resolve(Type t)
        {
            if (t.IsSubclassOf(typeof(ConfigBase)))
                return ConfigBase.GetConfig(t);

            string key = t.Assembly.FullName;
            if (types.ContainsKey(key))
                return ConfigBase.GetConfig(types[key]);

            return null;
        }
    }
}
