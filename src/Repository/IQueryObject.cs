﻿using System;

namespace Kiss
{
    /// <summary>
    /// 标记类为可查询
    /// </summary>
    public interface IQueryObject
    {
    }

    /// <summary>
    /// 标记属性为唯一值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PKAttribute : Attribute
    {
        public PKAttribute()
        {
            AutoIncrement = true;
        }

        /// <summary>
        /// 自增长，默认为true
        /// </summary>
        public bool AutoIncrement { get; set; }
    }

    /// <summary>
    /// 标记忽略该属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// Use this to mark if table/field name is different than class/property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class OriginalNameAttribute : Attribute
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public OriginalNameAttribute(string name)
        {
            this.name = name;
        }

        private readonly string name = string.Empty;

        /// <summary>
        /// maps to the name of the original name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
