using System;
using System.Collections.Generic;
using Kiss.Caching;
using Kiss.Utils;

namespace Kiss
{
    /// <summary>
    /// 缓存操作的入口
    /// </summary>
    public static class JCache
    {
        /// <summary>
        /// 将对象插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Insert(string key, object obj)
        {
            if (CacheConfig.Enabled && CacheConfig.IsEnabled(key))
                Insert(key, obj, CacheConfig.ValidFor);
        }

        /// <summary>
        /// 将对象插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="validFor"></param>
        public static void Insert(string key, object obj, TimeSpan validFor)
        {
            if (CacheConfig.Enabled && CacheConfig.IsEnabled(key))
                CacheConfig.Provider.Insert(GetCacheKey(key), obj, validFor);
        }

        /// <summary>
        /// 从缓存获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            if (CacheConfig.Enabled && CacheConfig.IsEnabled(key))
                return CacheConfig.Provider.Get(GetCacheKey(key));

            return null;
        }

        /// <summary>
        /// 从缓存获取对象列表
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IDictionary<string, object> Get(IEnumerable<string> keys)
        {
            if (CacheConfig.Enabled)
                return CacheConfig.Provider.Get(StringUtil.ToStringArray<string>(new List<string>(keys).ToArray(), delegate(string key) { return GetCacheKey(key); }));

            return null;
        }

        /// <summary>
        /// 从缓存获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (CacheConfig.Enabled)
                return (T)Get(key);

            return default(T);
        }

        /// <summary>
        /// 移除缓存中的对象
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (CacheConfig.Enabled && CacheConfig.IsEnabled(key))
                CacheConfig.Provider.Remove(GetCacheKey(key));
        }

        public static string GetRootCacheKey(string model)
        {
            return string.Format("{0}.root", model);
        }

        private static string GetCacheKey(string key)
        {
            return string.Format("{0}.{1}", CacheConfig.Namespace, key);
        }
    }
}