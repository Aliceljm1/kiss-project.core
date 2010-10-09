using System;
using System.Collections.Generic;
using System.Xml;

namespace Kiss.Plugin
{
    public class PluginSetting : ExtendedAttributes
    {
        internal PluginSetting()
        {
        }

        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enable { get; set; }
        public XmlNode Node { get; set; }
    }

    /// <summary>
    /// settings of plugin
    /// </summary>
    public class PluginSettings : List<PluginSetting>
    {
        internal PluginSettings()
        {
        }

        internal PluginSetting FindByName(string name)
        {
            PluginSetting setting = Find(delegate(PluginSetting s) { return string.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase); });
            if (setting == null)
                setting = new PluginSetting()
                {
                    Name = name,
                    Enable = true
                };

            return setting;
        }

        /// <summary>
        /// Find settings of the plugin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PluginSetting Get<T>() where T : IPluginInitializer
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// get plugin settings of specified plugin name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PluginSetting GetByPluginName(string name)
        {
            return PluginConfig.Instance.Settings.FindByName(name);
        }

        static PluginSetting Get(Type type)
        {
            PluginBootstrapper boot = ServiceLocator.Instance.Resolve<PluginBootstrapper>();

            if (boot._pluginSettings.ContainsKey(type.Name))
                return boot._pluginSettings[type.Name];

            return null;
        }
    }
}
