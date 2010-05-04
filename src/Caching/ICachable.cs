using System;

namespace Kiss.Caching
{
    /// <summary>
    /// 可缓存的对象, 如果ParentCacheKey非空, 缓存的对象是有关联的
    /// 清除缓存会清除所有相关的缓存
    /// </summary>
    public interface ICachable
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        string CacheKey { get; }

        /// <summary>
        /// 父缓存键
        /// </summary>
        string ParentCacheKey { get; }

        /// <summary>
        /// 数目缓存键
        /// </summary>
        string CountCacheKey { get; }

        /// <summary>
        /// 缓存有效期
        /// </summary>
        TimeSpan CacheTime { get; }
    }
}
