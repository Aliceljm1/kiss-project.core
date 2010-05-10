using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Caching
{
    /// <summary>
    /// cache config
    /// </summary>
    public static class CacheConfig
    {
        private static readonly PluginSetting setting = PluginSettings.Get<CacheInitializer>();

        /// <summary>
        /// cache valid days
        /// </summary>
        public static int CacheDay { get { return setting["cacheDay"].ToInt(1); } }

        public static bool Enabled { get { return setting.Enable && StringUtil.HasText(setting["type"]); } }

        /// <summary>
        /// cache key's namespace
        /// </summary>
        public static string Namespace { get { return setting["namespace"]; } }

        /// <summary>
        /// cache valid times
        /// </summary>
        public static TimeSpan ValidFor { get { return TimeSpan.FromDays(CacheDay); } }

        public static ICacheProvider Provider
        {
            get
            {
                return ServiceLocator.Instance.Resolve<ICacheProvider>();
            }
        }
    }
}
