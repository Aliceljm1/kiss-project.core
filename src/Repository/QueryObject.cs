using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Kiss.Query;
using Kiss.Utils;

namespace Kiss
{
    public abstract class QueryObject<T, t> : Obj<t>, IQueryObject
        where T : Obj<t>
    {
        public static T Get(t id)
        {
            return Repository.Get(id);
        }

        /// <summary>
        /// get obj list
        /// </summary>
        public static List<T> Gets(t[] ids)
        {
            return Repository.Gets(ids);
        }

        public static T Save(string param, ConvertObj<T> converter)
        {
            return Repository.Save(param, converter);
        }

        public static T Save(NameValueCollection param, ConvertObj<T> converter)
        {
            return Repository.Save(param, converter);
        }

        public static List<T> Gets(string commaDelimitedIds)
        {
            return Repository.Gets(commaDelimitedIds);
        }

        public static List<T> Gets(QueryCondition q)
        {
            return Repository.Gets(q);
        }

        public static int Count(QueryCondition q)
        {
            return Repository.Count(q);
        }

        public static T Save(T obj)
        {
            return Repository.Save(obj);
        }

        public static void DeleteById(params t[] ids)
        {
            Repository.DeleteById(ids);
        }

        public static IRepository<T, t> Repository
        {
            get
            {
                return QueryObject.GetRepository<T, t>();
            }
        }
    }

    public abstract class QueryObject<T> : IQueryObject where T : IQueryObject
    {
        public static List<T> Gets(QueryCondition q)
        {
            return Repository.Gets(q);
        }

        public static int Count(QueryCondition q)
        {
            return Repository.Count(q);
        }

        public static T Save(T obj)
        {
            return Repository.Save(obj);
        }

        public static IRepository<T> Repository
        {
            get
            {
                return QueryObject.GetRepository<T>();
            }
        }
    }

    public abstract class QueryObject : IQueryObject
    {
        public static T Get<T, t>(t id) where T : Obj<t>
        {
            return GetRepository<T, t>().Get(id);
        }

        /// <summary>
        /// get obj list
        /// </summary>
        public static List<T> Gets<T, t>(t[] ids) where T : Obj<t>
        {
            return GetRepository<T, t>().Gets(ids);
        }

        public static T Save<T, t>(string param, ConvertObj<T> converter) where T : Obj<t>
        {
            return GetRepository<T, t>().Save(param, converter);
        }

        public static T Save<T, t>(NameValueCollection param, ConvertObj<T> converter) where T : Obj<t>
        {
            return GetRepository<T, t>().Save(param, converter);
        }

        public static List<T> Gets<T, t>(string commaDelimitedIds) where T : Obj<t>
        {
            return GetRepository<T, t>().Gets(commaDelimitedIds);
        }

        public static List<T> Gets<T>(QueryCondition qc) where T : IQueryObject
        {
            return GetRepository<T>().Gets(qc);
        }

        public static int Count<T>(QueryCondition q) where T : IQueryObject
        {
            return GetRepository<T>().Count(q);
        }

        public static T Save<T>(T obj) where T : IQueryObject
        {
            return GetRepository<T>().Save(obj);
        }

        public static IRepository<T, t> GetRepository<T, t>() where T : Obj<t>
        {
            return ServiceLocator.Instance.Resolve<IRepository<T, t>>();
        }

        public static IRepository<T> GetRepository<T>() where T : IQueryObject
        {
            return ServiceLocator.Instance.Resolve<IRepository<T>>();
        }

        public static IRepository GetRepository(Type type)
        {
            if (type.GetInterface("IQueryObject") == null)
                throw new KissException("type {0} is not inherite from Kiss.IQueryObject", type.FullName);

            Type t2 = typeof(IRepository<>).MakeGenericType(type);

            return ServiceLocator.Instance.Resolve(t2) as IRepository;
        }

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
        public static void OnSaved(IQueryObject obj, SaveAction action)
        {
            if (Saved != null)
            {
                Saved(obj, new SavedEventArgs(action));
            }
        }

        /// <summary>
        /// Raises the Saving event
        /// </summary>
        public static void OnSaving(IQueryObject obj, SavingEventArgs e)
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
                    tableName = GetTableName(GetType());

                return tableName;
            }
        }

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

        public static string GetTableName<T>() where T : IQueryObject
        {
            return GetTableName(typeof(T));
        }
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
}
