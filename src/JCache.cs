#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    JCache.cs
//+ File Created:   20090729
//+-------------------------------------------------------------------+
//+ Purpose:        JCache定义
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI Comment Created
//+-------------------------------------------------------------------+
//+ 20090903        ZHLI 增加了对是否启用缓存的判断
//+-------------------------------------------------------------------+
//+ 20090904        ZHLI fix a bug of Get<T> method
//+-------------------------------------------------------------------+
#endregion

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
            if (CacheConfig.Enabled)
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
            if (CacheConfig.Enabled)
                CacheConfig.Provider.Insert(GetCacheKey(key), obj, validFor);
        }

        /// <summary>
        /// 从缓存获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {           
            if (CacheConfig.Enabled)
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

            if (CacheConfig.Enabled)
                CacheConfig.Provider.Remove(GetCacheKey(key));
        }

        private static string GetCacheKey(string key)
        {
            return string.Format("{0}.{1}", CacheConfig.Namespace, key);
        }
    }
}
