using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss
{
    [AutoInit(Title = "repository", Priority = 8)]
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
                    sl.AddComponent("kiss.repository_1", typeof(IRepository<>), t1, Castle.Core.LifestyleType.PerWebRequest);
                }
                if (StringUtil.HasText(type2))
                {
                    Type t2 = Type.GetType(type2, true, true);
                    sl.AddComponent("kiss.repository_2", typeof(IRepository<,>), t2, Castle.Core.LifestyleType.PerWebRequest);
                }
            }
        }

        #endregion
    }
}
