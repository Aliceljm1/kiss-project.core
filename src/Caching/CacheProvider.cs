using System;
using System.Collections.Generic;
using System.Configuration.Provider;

namespace Kiss.Caching
{
    /// <summary>
    /// abstract cace Provider
    /// </summary>
    public abstract class CacheProvider : ProviderBase
    {
        #region Instance

        /// <summary>
        /// 缓存实例
        /// </summary>
        public static CacheProvider Instance { get { return Singleton<CacheProvider>.Instance; } }

        static CacheProvider()
        {
            Singleton<CacheProvider>.Instance = ProviderHelper.CreateProvider(CacheConfig.Instance) as CacheProvider;
        }

        #endregion

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="validFor"></param>
        public abstract void Insert(string key, object obj, TimeSpan validFor);

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract object Get(string key);

        /// <summary>
        /// 获取缓存字典（批量获取）
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public abstract IDictionary<string, object> Get(IEnumerable<string> keys);

        /// <summary>
        /// 移除缓存值
        /// </summary>
        /// <param name="key"></param>
        public abstract void Remove(string key);
    }
}
