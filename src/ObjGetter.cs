#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    BizObjGetter.cs
//+ File Created:   20090730
//+-------------------------------------------------------------------+
//+ Purpose:        Get方法获取单个对象，Gets方法获取对象列表
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090730        ZHLI Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Collections.Generic;
using Kiss.Caching;
using Kiss.Utils;

namespace Kiss
{
    /// <summary>
    /// 使用该类获取对象（优先从缓存取）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="t">ID</typeparam>
    public static class ObjGetter<T, t>
        where T : Obj<t>
        where t : IEquatable<t>
    {
        /// <summary>
        /// 获取对象（优先从缓存取）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getCacheKey"></param>
        /// <param name="getBizObj"></param>
        /// <returns></returns>
        public static T Get ( t id, GetCacheKey<t> getCacheKey, GetObj<T, t> getBizObj )
        {
            if ( id == null || id.Equals ( default ( t ) ) )
                return null;

            if ( !CacheConfig.Instance.Enabled )
                return getBizObj ( id );

            string key = getCacheKey ( id );

            T obj = JCache.Get<T> ( key );

            if ( obj == null )
            {
                obj = getBizObj ( id );
                if ( obj != null )
                    JCache.Insert ( key, obj );
            }

            return obj;
        }

        /// <summary>
        /// 获取对象列表（优先从缓存取）
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="getCacheKey"></param>
        /// <param name="getsBizObj"></param>
        /// <returns></returns>
        public static List<T> Gets ( t[ ] ids, GetCacheKey<t> getCacheKey, GetsObj<T, t> getsBizObj )
        {
            if ( ids == null || ids.Length == 0 )
                return new List<T> ( 0 );

            if ( !CacheConfig.Instance.Enabled )
                return getsBizObj ( ids );

            // get from cache
            IDictionary<string,object> dict = JCache.Get ( StringUtil.ToStringArray<t> ( ids, delegate ( t id ) { return getCacheKey ( id ); } ) );

            // get uncached_id
            List<t> unCached_id = new List<t> ( );
            foreach ( t id in ids )
            {
                string key = getCacheKey ( id );
                if ( !dict.ContainsKey ( key ) )
                    unCached_id.Add ( id );
            }

            // get uncached from db
            Dictionary<t, T> unCached_list = new Dictionary<t, T> ( );
            if ( unCached_id.Count > 0 )
            {
                foreach ( T obj in getsBizObj ( ids ) )
                {
                    string key = getCacheKey ( obj.Id );
                    JCache.Insert ( key, obj );
                    unCached_list.Add ( obj.Id, obj );
                }
            }

            // combin to a list
            List<T> list = new List<T> ( ids.Length );
            foreach ( t id in ids )
            {
                T obj = null;

                string key = getCacheKey ( id );
                if ( dict.ContainsKey ( key ) )
                    obj = dict[ key ] as T;
                else if ( unCached_list.ContainsKey ( id ) )
                    obj = unCached_list[ id ];

                if ( obj != null )
                    list.Add ( obj );
            }

            return list;
        }
    }

    /// <summary>
    /// 获取缓存键
    /// </summary>
    /// <typeparam name="t"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public delegate string GetCacheKey<t> ( t id );

    /// <summary>
    /// 获取单个对象。（通常是从数据库获取）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="t"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public delegate T GetObj<T, t> ( t id );

    /// <summary>
    /// 获取对象列表。（通常是从数据库获取）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="t"></typeparam>
    /// <param name="ids"></param>
    /// <returns></returns>
    public delegate List<T> GetsObj<T, t> ( t[ ] ids );

    /// <summary>
    /// 使用该类获取对象（优先从缓存取）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public static class ObjGetter<T> where T : Obj
    {
        /// <summary>
        /// 获取对象（优先从缓存取）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="getCacheKey"></param>
        /// <param name="getBizObj"></param>
        /// <returns></returns>
        public static T Get ( int id, GetCacheKey getCacheKey, GetObj<T> getBizObj )
        {
            if ( id == 0 )
                return null;

            if ( !CacheConfig.Instance.Enabled )
                return getBizObj ( id );

            string key = getCacheKey ( id );

            T obj = JCache.Get<T> ( key );

            if ( obj == null )
            {
                obj = getBizObj ( id );
                if ( obj != null )
                    JCache.Insert ( key, obj );
            }

            return obj;
        }

        /// <summary>
        /// 获取对象列表（优先从缓存取）
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="getCacheKey"></param>
        /// <param name="getsBizObj"></param>
        /// <returns></returns>
        public static List<T> Gets ( int[ ] ids, GetCacheKey getCacheKey, GetsObj<T> getsBizObj )
        {
            if ( ids == null || ids.Length == 0 )
                return new List<T> ( 0 );

            if ( !CacheConfig.Instance.Enabled )
                return getsBizObj ( ids );

            // get from cache
            IDictionary<string,object> dict = JCache.Get ( StringUtil.ToStringArray<int> ( ids, delegate ( int id ) { return getCacheKey ( id ); } ) );

            // get uncached_id
            List<int> unCached_id = new List<int> ( );
            foreach ( int id in ids )
            {
                string key = getCacheKey ( id );
                if ( !dict.ContainsKey ( key ) )
                    unCached_id.Add ( id );
            }

            // get uncached from db
            Dictionary<int, T> unCached_list = new Dictionary<int, T> ( );
            if ( unCached_id.Count > 0 )
            {
                foreach ( T obj in getsBizObj ( ids ) )
                {
                    string key = getCacheKey ( obj.Id );
                    JCache.Insert ( key, obj );
                    unCached_list.Add ( obj.Id, obj );
                }
            }

            // combin to a list
            List<T> list = new List<T> ( ids.Length );
            foreach ( int id in ids )
            {
                T obj = null;

                string key = getCacheKey ( id );
                if ( dict.ContainsKey ( key ) )
                    obj = dict[ key ] as T;
                else if ( unCached_list.ContainsKey ( id ) )
                    obj = unCached_list[ id ];

                if ( obj != null )
                    list.Add ( obj );
            }

            return list;
        }
    }

    /// <summary>
    /// 获取缓存键
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public delegate string GetCacheKey ( int id );

    /// <summary>
    /// 获取单个对象。（通常是从数据库获取）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public delegate T GetObj<T> ( int id );

    /// <summary>
    /// 获取对象列表。（通常是从数据库获取）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ids"></param>
    /// <returns></returns>
    public delegate List<T> GetsObj<T> ( int[ ] ids );
}
