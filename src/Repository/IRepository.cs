using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Kiss.Query;

namespace Kiss
{
    public delegate bool ConvertObj<T>(T obj, NameValueCollection param);

    /// <summary>
    /// obj's basic operations
    /// </summary>
    public interface IRepository<T, t> : IRepository<T>
        where T : Obj<t>
    {
        /// <summary>
        /// get single obj by key
        /// </summary>
        T Get(t id);

        /// <summary>
        /// get obj list
        /// </summary>
        List<T> Gets(t[] ids);

        T Save(string param, ConvertObj<T> converter);

        T Save(NameValueCollection param, ConvertObj<T> converter);

        List<T> Gets(string commaDelimitedIds);

        void DeleteById(params t[] ids);
    }

    public interface IRepository<T> : IRepository
        where T : IQueryObject
    {
        new List<T> Gets(QueryCondition q);

        T Save(T obj);

        ILinqQuery<T> Query { get; }
    }

    public interface IRepository
    {
        object Gets(QueryCondition q);

        int Count(QueryCondition q);

        ConnectionStringSettings ConnectionStringSettings { get; set; }
    }

    public interface ILinqQuery<T> : IOrderedQueryable<T>, IQueryProvider
    {
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        void AddRange(IEnumerable<T> items, bool inMemorySort);
        void Remove(T value);
        void SubmitChanges();
        void SubmitChanges(bool batch);
        bool EnableQueryEvent { get; set; }
    }
}
