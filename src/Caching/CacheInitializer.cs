using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Caching
{
    /// <summary>
    /// cache initializer. use this class to create cache provider
    /// </summary>
    [AutoInit(Title = "Cache", Priority = 10)]
    class CacheInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, ref PluginSetting setting)
        {
            if (!setting.Enable) return;

            string type = setting["type"];
            if (StringUtil.IsNullOrEmpty(type))
                setting.Enable = false;
            else
                sl.AddComponent("kiss.cache", typeof(ICacheProvider), Type.GetType(type, true, true));
        }

        #endregion
    }
}
