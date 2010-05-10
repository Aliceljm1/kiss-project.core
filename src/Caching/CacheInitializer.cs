using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Caching
{
    /// <summary>
    /// cache initializer. use this class to create cache provider
    /// </summary>
    [AutoInit(Title = "cache", Priority = 10)]
    public class CacheInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, PluginSetting setting)
        {
            if (setting.Enable)
            {
                string type = setting["type"];
                if (StringUtil.IsNullOrEmpty(type))
                    setting.Enable = false;
                else
                    sl.AddComponent("kiss.cache", typeof(ICacheProvider), Type.GetType(type, true, true));
            }
        }

        #endregion
    }
}
