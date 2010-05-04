#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:   20090827
//+-------------------------------------------------------------------+
//+ Purpose:        关系查询Provider
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090827        ZHLI Comment Created
//+-------------------------------------------------------------------+
//+ 20090903        ZHLI 转化为接口，自动根据创建合适的Provider
//+-------------------------------------------------------------------+
#endregion

using System.Collections.Generic;
using System.Data;

namespace Kiss.Query
{
    /// <summary>
    /// 关系查询抽象Provider
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Count(QueryCondition condition);

        /// <summary>
        /// 获取记录主键列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<T> GetRelationIds<T>(QueryCondition condition);

        /// <summary>
        /// 获取IDataReader
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        IDataReader GetReader(QueryCondition q);

        /// <summary>
        /// delete data using query condition
        /// </summary>
        /// <param name="q"></param>
        void Delete(QueryCondition q);
    }
}