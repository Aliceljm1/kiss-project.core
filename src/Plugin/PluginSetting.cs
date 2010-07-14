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

        public static PluginSetting Get<T>() where T : IPluginInitializer
        {
            PluginBootstrapper boot = ServiceLocator.Instance.Resolve<PluginBootstrapper>();

            Type t = typeof(T);

            if (boot._pluginSettings.ContainsKey(t.Name))
                return boot._pluginSettings[t.Name];

            return null;
        }
    }
}
