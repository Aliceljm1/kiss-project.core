using System.Collections.Generic;
using System.Collections.Specialized;

namespace Kiss
{
    public enum CreditActionResult
    {
        /// <summary>
        /// 执行成功
        /// </summary>
        PointsSuccess,

        /// <summary>
        /// 没有任何有效的规则
        /// </summary>
        PointsNoRule,

        /// <summary>
        /// 积分不足,不能执行
        /// </summary>
        PointsLacking
    }

    public interface ICredit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActionName">动作名称</param>
        /// <param name="UserId">指定给谁加/减积分</param>
        /// <returns>执行结果</returns>
        CreditActionResult TriggerAction(string ActionName, string UserId);

        /// <summary>
        /// 触发积分动作
        /// </summary>
        /// <param name="ActionName">动作名称</param>
        /// <param name="UserId">指定给谁加/减积分</param>
        /// <param name="Description">备注信息</param>
        /// <returns>执行结果</returns>
        CreditActionResult TriggerAction(string ActionName, string UserId, string Description);

        /// <summary>
        /// 触发积分动作
        /// </summary>
        /// <param name="ActionName">动作名称</param>
        /// <param name="UserId">指定给谁加/减积分</param>
        /// <param name="Params">用来标志唯一的参数,如下载的文档ID</param>
        ///  <returns>执行结果</returns>
        CreditActionResult TriggerAction(string ActionName, string UserId, NameValueCollection Params);

        /// <summary>
        /// 触发积分动作
        /// </summary>
        /// <param name="ActionName"动作名称></param>
        /// <param name="UserId">指定给谁加/减积分</param>
        /// <param name="Description">备注信息</param>
        /// <param name="Params">用来标志唯一的参数,如下载的文档ID</param>
        ///  <returns>执行结果</returns>
        CreditActionResult TriggerAction(string ActionName, string UserId, string Description, NameValueCollection Params);

        /// <summary>
        /// 获取用户的积分等级
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <returns>积分等级</returns>
        NameValueCollection getUserPoints(string UserId);

        /// <summary>
        /// 返回所有的CreditRules插件
        /// </summary>
        List<ICreditRule> CreditRules { get; }

        /// <summary>
        /// 获取日志IWhere对象
        /// </summary>
        /// <param name="where"></param>
        /// <param name="Par"></param>
        /// <returns></returns>
        IWhere WhereCreditLog(string where, params object[] Par);

        /// <summary>
        /// 获取只查询item日志IWhere对象
        /// </summary>
        /// <param name="item"></param>
        /// <param name="where"></param>
        /// <param name="Par"></param>
        /// <returns></returns>
        IWhere WhereCreditLog(ICreditRule item, string where, params object[] Par);
    }
}