using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Kiss.Query;
using Kiss.Utils;

namespace Kiss
{
    [Serializable]
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

        public static List<T> GetsAll()
        {
            return Repository.GetsAll();
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

        public static string GetTableName()
        {
            return QueryObject.GetTableName<T>();
        }

        public static IRepository<T, t> Repository
        {
            get
            {
                return QueryObject.GetRepository<T, t>();
            }
        }

        public static IKissQueryable<T> Query { get { return Repository.Query; } }

        public static ConnectionStringSettings ConnectionStringSettings { get { return Repository.ConnectionStringSettings; } set { Repository.ConnectionStringSettings = value; } }
    }

    [Serializable]
    public abstract class QueryObject<T> : QueryObject, IQueryObject where T : IQueryObject
    {
        public static List<T> Gets(QueryCondition q)
        {
            return Repository.Gets(q);
        }

        public static List<T> GetsAll()
        {
            return Repository.GetsAll();
        }

        public static int Count(QueryCondition q)
        {
            return Repository.Count(q);
        }

        public static T Save(T obj)
        {
            return Repository.Save(obj);
        }

        public static string GetTableName()
        {
            return QueryObject.GetTableName<T>();
        }

        public static IRepository<T> Repository
        {
            get
            {
                return QueryObject.GetRepository<T>();
            }
        }

        public static IKissQueryable<T> Query { get { return Repository.Query; } }

        public static ConnectionStringSettings ConnectionStringSettings { get { return Repository.ConnectionStringSettings; } set { Repository.ConnectionStringSettings = value; } }
    }

    [Serializable]
    public abstract class QueryObject : IQueryObject
    {
        internal static IRepository<T, t> GetRepository<T, t>() where T : Obj<t>
        {
            return ServiceLocator.Instance.Resolve<IRepository<T, t>>();
        }

        internal static IRepository<T> GetRepository<T>() where T : IQueryObject
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

        /// <summary>
        /// pre query event
        /// </summary>
        public static event EventHandler<QueryEventArgs> PreQuery;

        public static void OnPreQuery(QueryEventArgs e)
        {
            EventHandler<QueryEventArgs> handler = PreQuery;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        public class QueryEventArgs : EventArgs
        {
            public static readonly new QueryEventArgs Empty = new QueryEventArgs();

            public Type Type { get; set; }
            public string TableName { get; set; }
            public string Sql { get; set; }
            public object Result { get; set; }
        }

        /// <summary>
        /// after query event
        /// </summary>
        public static event EventHandler<QueryEventArgs> AfterQuery;

        public static void OnAfterQuery(QueryEventArgs e)
        {
            EventHandler<QueryEventArgs> handler = AfterQuery;

            if (handler != null)
            {
                handler(null, e);
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
            object[] attr = type.GetCustomAttributes(typeof(OriginalNameAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                OriginalNameAttribute originalEntityNameAtt = attr[0] as OriginalNameAttribute;
                return originalEntityNameAtt.Name;
            }

            return type.Name;
        }

        internal static string GetTableName<T>() where T : IQueryObject
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
        public string TableName { get; set; }
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
