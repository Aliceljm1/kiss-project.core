using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Caching
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public static class CacheConfig
    {
        private static readonly PluginSetting setting = PluginSettings.Get<CacheInitializer>();

        /// <summary>
        /// cache valid days
        /// </summary>
        public static int CacheDay { get { return setting["cacheDay"].ToInt(1); } }

        /// <summary>
        /// 是否启用
        /// </summary>
        public static bool Enabled { get { return setting.Enable && StringUtil.HasText(setting["type"]); } }

        private static readonly string[] ignores = StringUtil.Split(setting["ignores"], ",", true, true);

        /// <summary>
        /// 判断指定的缓存key是否启用
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static bool IsEnabled(string cacheKey)
        {
            foreach (var item in ignores)
            {
                if (cacheKey.StartsWith(item, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// cache key's namespace
        /// </summary>
        public static string Namespace { get { return setting["namespace"]; } }

        /// <summary>
        /// cache valid times
        /// </summary>
        public static TimeSpan ValidFor { get { return TimeSpan.FromDays(CacheDay); } }

        /// <summary>
        /// 缓存provider
        /// </summary>
        public static ICacheProvider Provider
        {
            get
            {
                return ServiceLocator.Instance.Resolve<ICacheProvider>();
            }
        }
    }
}
