using System;
using System.Collections.Specialized;

namespace Kiss
{
    public interface ILicenceProvider
    {
        bool Check();

        NameValueCollection GetLicenseDetail();

        /// <summary>
        /// 校验licence失败后的回调函数
        /// </summary>
        /// <returns>返回true，继续执行；返回false，中断执行</returns>
        bool OnLicenceInvalid();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class CheckLicenceAttribute : Attribute
    {
    }
}
