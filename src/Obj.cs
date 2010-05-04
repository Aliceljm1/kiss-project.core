#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    Obj.cs
//+ File Created:   20090729
//+-------------------------------------------------------------------+
//+ Purpose:        BizObj定义
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using Kiss.Utils;

namespace Kiss
{
    /// <summary>
    /// 业务实体基类，所有的业务实体类从此类继承
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Obj<T> : IQueryObject
    {
        #region props

        /// <summary>
        /// Id
        /// </summary>
        [UniqueIdentifier]
        public T Id { get; set; }

        /// <summary>
        /// 是否是新创建对象
        /// </summary>
        [Ignore]
        public bool IsNew { get { return object.Equals(Id, default(T)); } }

        private string tableName;

        /// <summary>
        /// 对象对应表名称
        /// </summary>
        [Ignore]
        public string TableName
        {
            get
            {
                if (StringUtil.IsNullOrEmpty(tableName))
                {
                    object[] attr = GetType().GetCustomAttributes(typeof(OriginalEntityNameAttribute), true);
                    if (attr != null && attr.Length > 0)
                    {
                        OriginalEntityNameAttribute originalEntityNameAtt = attr[0] as OriginalEntityNameAttribute;
                        tableName = originalEntityNameAtt.EntityName;
                    }
                    else
                    {
                        tableName = GetType().Name;
                    }
                }

                return tableName;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Obj()
        {
        }

        /// <summary>
        /// 从IDataReader创建对象
        /// </summary>
        /// <param name="rdr"></param>
        public Obj(IDataReader rdr)
        {
            Id = (T)rdr["Id"];
        }

        #endregion

        #region equality overrides

        /// <summary>
        /// A uniquely key to identify this particullar instance of the class
        /// </summary>
        /// <returns>A unique integer value</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Comapares this object with another
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the two objects as equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() == this.GetType())
            {
                return obj.GetHashCode() == this.GetHashCode();
            }

            return false;
        }

        /// <summary>
        /// Checks to see if two business objects are the same.
        /// </summary>
        public static bool operator ==(Obj<T> first, Obj<T> second)
        {
            if (Object.ReferenceEquals(first, second))
            {
                return true;
            }

            if ((object)first == null || (object)second == null)
            {
                return false;
            }

            return first.GetHashCode() == second.GetHashCode();
        }

        /// <summary>
        /// Checks to see if two business objects are different.
        /// </summary>
        public static bool operator !=(Obj<T> first, Obj<T> second)
        {
            return !(first == second);
        }

        #endregion

        #region events

        /// <summary>
        /// Occurs when the obj is Saved
        /// </summary>
        public static event EventHandler<SavedEventArgs> Saved;

        /// <summary>
        /// Occurs when the class is Saving
        /// </summary>
        public static event EventHandler<SavingEventArgs> Saving;

        /// <summary>
        /// Raises the Saved event.
        /// </summary>
        public static void OnSaved(Obj<T> obj, SaveAction action)
        {
            if (Saved != null)
            {
                Saved(obj, new SavedEventArgs(action));
            }
        }

        /// <summary>
        /// Raises the Saving event
        /// </summary>
        public static void OnSaving(Obj<T> obj, SavingEventArgs e)
        {
            if (Saving != null)
            {
                Saving(obj, e);
            }
        }

        /// <summary>
        /// Raise the batch add/update/create event
        /// </summary>
        public static event EventHandler<BatchEventArgs> Batch;

        public static void OnBatch(Type type)
        {
            EventHandler<BatchEventArgs> handler = Batch;

            if (handler != null)
            {
                handler(null, new BatchEventArgs() { Type = type });
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// get table name of object type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            object[] attr = type.GetCustomAttributes(typeof(OriginalEntityNameAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                OriginalEntityNameAttribute originalEntityNameAtt = attr[0] as OriginalEntityNameAttribute;
                return originalEntityNameAtt.EntityName;
            }

            return type.Name;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class SavedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public SavedEventArgs(SaveAction action)
        {
            Action = action;
        }

        private SaveAction _Action;
        /// <summary>
        /// Gets or sets the action that occured when the object was saved.
        /// </summary>
        public SaveAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SavingEventArgs : SavedEventArgs
    {
        /// <summary>
        /// if or not cancel saving
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public SavingEventArgs()
            : base(SaveAction.None)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="action"></param>
        public SavingEventArgs(SaveAction action)
            : base(action)
        {
            Cancel = false;
        }
    }

    public class BatchEventArgs : EventArgs
    {
        public static readonly new BatchEventArgs Empty = new BatchEventArgs();
        public Type Type { get; set; }
    }

    /// <summary>
    /// The action performed by the save event.
    /// </summary>
    public enum SaveAction
    {
        /// <summary>
        /// Default. Nothing happened.
        /// </summary>
        None,
        /// <summary>
        /// It's a new object that has been inserted.
        /// </summary>
        Insert,
        /// <summary>
        /// It's an old object that has been updated.
        /// </summary>
        Update,
        /// <summary>
        /// The object was deleted.
        /// </summary>
        Delete
    }

    /// <summary>
    /// 业务实体基类，主键是<see cref="int"/>类型
    /// </summary>
    [Serializable]
    public abstract class Obj : Obj<int>
    {
        public Obj()
        {
        }

        public Obj(IDataReader rdr)
            : base(rdr)
        {
        }
    }

    /// <summary>
    /// obj support extend properties
    /// </summary>
    [Serializable]
    public abstract class ExtendObj : Obj
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }

        private ExtendedAttributes _extAttrs;
        [Ignore]
        public ExtendedAttributes ExtAttrs
        {
            get
            {
                if (_extAttrs == null)
                {
                    _extAttrs = new ExtendedAttributes();
                    _extAttrs.SetData(PropertyName, PropertyValue);
                }
                return _extAttrs;
            }
        }

        [Ignore]
        public string this[string key]
        {
            get
            {
                if (ExtAttrs.ExtendedAttributesCount == 0)
                    ExtAttrs.SetData(PropertyName, PropertyValue);
                return ExtAttrs.GetExtendedAttribute(key);
            }
            set
            {
                ExtAttrs.SetExtendedAttribute(key, value);
            }
        }

        public void SerializeExtAttrs()
        {
            SerializerData sd = ExtAttrs.GetSerializerData();

            PropertyName = sd.Keys;
            PropertyValue = sd.Values;
        }
    }

    /// <summary>
    /// dict schema
    /// </summary>
    [Serializable]
    [OriginalEntityName("gDictSchema")]
    public class DictSchema : ExtendObj, IComparable<DictSchema>
    {
        public int SiteId { get; set; }
        public int ParentId { get; set; }
        public int Depth { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool HasChild { get; set; }
        public bool IsValid { get; set; }

        [Ignore]
        public DictSchema Parent { get; set; }
        [Ignore]
        public List<DictSchema> Children { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public int CompareTo(DictSchema other)
        {
            if (other == null)
                return 1;
            if (Id == other.Id)
                return 0;

            int result = Depth.CompareTo(other.Depth);
            if (result == 0)
            {
                DictSchema p1 = this;
                DictSchema p2 = other;
                while (p1.Parent != p2.Parent)
                {
                    p1 = p1.Parent;
                    p2 = p2.Parent;
                }
                result = p1.SortOrder.CompareTo(p2.SortOrder);
            }
            else
            {
                DictSchema s = result > 0 ? this : other;
                int d = Math.Abs(Depth - other.Depth);
                while (d-- > 0)
                {
                    s = s.Parent;
                }
                if (result > 0)
                {
                    if (s.Id != other.Id)
                        result = s.CompareTo(other);
                }
                else
                {
                    if (s.Id != Id)
                        result = this.CompareTo(s);
                }
            }

            return result;
        }
    }
}
