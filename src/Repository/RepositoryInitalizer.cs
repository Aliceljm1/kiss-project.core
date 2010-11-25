using System;
using System.Xml;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss
{
    [AutoInit(Title = "repository", Priority = 9)]
    public class RepositoryInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, PluginSetting setting)
        {
            if (!setting.Enable)
                return;

            string type1 = setting["type1"];
            string type2 = setting["type2"];
            if (StringUtil.IsNullOrEmpty(type1) && StringUtil.IsNullOrEmpty(type2))
                setting.Enable = false;
            else
            {
                if (StringUtil.HasText(type1))
                {
                    Type t1 = Type.GetType(type1, true, true);
                    sl.AddComponent("kiss.repository_1", typeof(IRepository<>), t1);
                }
                if (StringUtil.HasText(type2))
                {
                    Type t2 = Type.GetType(type2, true, true);
                    sl.AddComponent("kiss.repository_2", typeof(IRepository<,>), t2);
                }
            }

            if (setting.Node != null)
            {
                // providers
                foreach (XmlNode provider in setting.Node.SelectNodes("providers/add"))
                {
                    bool enabled = XmlUtil.GetBoolAttribute(provider, "enable", true);

                    if (!enabled) continue;
                    string name = XmlUtil.GetStringAttribute(provider, "name", string.Empty);
                    if (string.IsNullOrEmpty(name))
                        continue;

                    Type type = Type.GetType(XmlUtil.GetStringAttribute(provider, "type", string.Empty), true, false);
                    if (type == null) continue;

                    sl.AddComponent(name, type);
                }
            }
        }

        #endregion
    }
}
