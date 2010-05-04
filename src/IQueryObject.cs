#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    IQueryObject.cs
//+ File Created:   20090729
//+-------------------------------------------------------------------+
//+ Purpose:        IQueryObject定义
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI Comment Created
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI 添加OriginalEntityNameAttribute, OriginalFieldNameAttribute
//+-------------------------------------------------------------------+
#endregion

using System;

namespace Kiss
{
    /// <summary>
    /// 标记类为可查询类
    /// </summary>
    public interface IQueryObject
    {
    }

    /// <summary>
    /// 标记属性为唯一值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueIdentifierAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记忽略该属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// Use this to mark if entity name is different than class name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OriginalEntityNameAttribute : Attribute
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="entityName"></param>
        public OriginalEntityNameAttribute(string entityName)
        {
            this.entityName = entityName;
        }

        private readonly string entityName = string.Empty;

        /// <summary>
        /// maps to the name of the original enity name.
        /// </summary>
        public string EntityName
        {
            get
            {
                return entityName;
            }
        }
    }

    /// <summary>
    /// use this to mark if field name is different than property name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OriginalFieldNameAttribute : Attribute
    {
        /// <summary>
        /// field name in database
        /// </summary>
        public string FieldName
        {
            get
            {
                return fieldName;
            }
        }

        private readonly string fieldName = string.Empty;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="fieldName"></param>
        public OriginalFieldNameAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }
}
