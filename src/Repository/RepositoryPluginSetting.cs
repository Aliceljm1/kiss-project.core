using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Repository
{
    public class RepositoryPluginSetting : PluginSettingDecorator
    {
        public string DefaultConn { get; private set; }
        public Dictionary<string, string> Conns { get; private set; }

        public RepositoryPluginSetting(PluginSetting setting)
            : base(setting)
        {
            // conns
            Conns = new Dictionary<string, string>();

            if (setting.Node != null)
            {
                XmlNode connsnode = setting.Node.SelectSingleNode("conns");

                if (connsnode != null)
                {
                    DefaultConn = XmlUtil.GetStringAttribute(connsnode, "default", string.Empty);

                    foreach (XmlNode conn in connsnode.ChildNodes)
                    {
                        string conn_name = XmlUtil.GetStringAttribute(conn, "conn", string.Empty);
                        if (string.IsNullOrEmpty(conn_name)) continue;

                        string table = XmlUtil.GetStringAttribute(conn, "table", string.Empty);
                        if (string.IsNullOrEmpty(table)) continue;

                        Conns[conn_name] = table;
                    }
                }
            }

            // use first connection string if not config
            if (string.IsNullOrEmpty(DefaultConn) && ConfigurationManager.ConnectionStrings.Count > 0)
                DefaultConn = ConfigurationManager.ConnectionStrings[0].Name;            
        }
    }
}
