#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:   2009-09-17
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 2009-09-17		zhli Comment Created
//+-------------------------------------------------------------------+
#endregion

using System.Collections.Generic;
using System.Collections.Specialized;
using Kiss.Query;

namespace Kiss
{
    /// <summary>
    /// object manager
    /// </summary>
    public interface IObjManager
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        void Delete(params int[] ids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="q"></param>
        void Delete(QueryCondition q);

        int Count();

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        int Count(QueryCondition qc);

        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        object Get(int id);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        object Gets(int[] ids);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        object Gets(QueryCondition qc);

        object GetsPaged(int pageIndex, int pageCount);
    }

    /// <summary>
    /// 对象的CRUD操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjManager<T> : IObjManager
        where T : Obj, new()
    {
        /// <summary>
        /// 获取单个对象
        /// </summary>
        new T Get(int id);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        new List<T> Gets(int[] ids);

        /// <summary>
        /// 获取对象列表
        /// </summary>
        new List<T> Gets(QueryCondition qc);

        new List<T> GetsPaged(int pageIndex, int pageCount);

        /// <summary>
        /// 新建对象
        /// </summary>
        T Create(T obj);

        /// <summary>
        /// 保存对象
        /// </summary>
        T Save(string param, ConvertObj<T> converter);

        /// <summary>
        /// 保存对象
        /// </summary>
        T Save(NameValueCollection param, ConvertObj<T> converter);
    }

    /// <summary>
    /// 从NameValueCollection转换为实体对象
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public delegate bool ConvertObj<T>(T obj, NameValueCollection param);
}
