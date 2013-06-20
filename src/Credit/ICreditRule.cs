using System.Collections.Specialized;

namespace Kiss
{
    public interface ICreditRule
    {
        /// <summary>
        /// 获取规则的名称
        /// </summary>
        string RuleName { get; }

        /// <summary>
        /// 获取动作名称
        /// </summary>
        string ActionName { get; }

        /// <summary>
        /// 返回此积分规则是否满足条件
        /// </summary>
        /// <returns></returns>
        bool CheckValid(string UserId, NameValueCollection Params);
    }
}