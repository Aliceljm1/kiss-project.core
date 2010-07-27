using System;
using System.Collections.Generic;

namespace Kiss.Caching
{
    /// <summary>
    /// abstract cache Provider
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 插入缓存
        /// </summary>
        void Insert(string key, object obj, TimeSpan validFor);

        /// <summary>
        /// 从缓存中获取值
        /// </summary>
        object Get(string key);

        /// <summary>
        /// 从缓存中批量获取值
        /// </summary>
        IDictionary<string, object> Get(IEnumerable<string> keys);

        /// <summary>
        /// 从缓存中移除值
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
