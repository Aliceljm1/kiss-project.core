using System.Xml;
using Kiss.Config;
using Kiss.Utils;

namespace Kiss.Plugin
{
    internal class PluginConfig : ConfigBase
    {
        public static PluginConfig Instance { get { return GetConfig<PluginConfig>("plugins", true); } }

        public PluginSettings Settings = new PluginSettings();

        protected override void LoadValuesFromConfigurationXml(XmlNode node)
        {
            base.LoadValuesFromConfigurationXml(node);

            foreach (XmlNode n in node.SelectNodes("plugin"))
            {
                string name = XmlUtil.GetStringAttribute(n, "name", string.Empty);
                if (string.IsNullOrEmpty(name))
                    continue;

                PluginSetting s = new PluginSetting()
                {
                    Name = name,
                    Enable = XmlUtil.GetBoolAttribute(n, "enable", true),
                    Node = n
                };

                foreach (XmlAttribute attr in n.Attributes)
                {
                    s[attr.Name] = attr.Value;
                }

                Settings.Add(s);
            }
        }
    }
}
