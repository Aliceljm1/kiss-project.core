using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using Kiss.Caching;
using Kiss.Config;
using Kiss.Utils;

namespace Kiss.Query
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class QueryCondition : ExtendedAttributes, ICachable
    {
        #region ctor

        public QueryCondition()
        {
        }

        public QueryCondition(string connstr_name)
        {
            SetConnectionStringName(connstr_name);
        }

        #endregion

        #region props

        /// <summary>
        /// query id
        /// </summary>
        public string Id { get; private set; }

        public string Keyword { get; set; }

        public int PageIndex { get; set; }

        private int _pageSize = 20;

        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

        public virtual bool Paging { get { return PageSize > 0; } }

        public int PageCount { get { return TotalCount / PageSize; } }

        public int PageIndex1 { get { return PageIndex + 1; } }

        public int TotalCount { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                if (ConnectionStringSettings == null)
                    throw new QueryException("ConnectionStringSettings is not set.");
                return ConnectionStringSettings.ConnectionString;
            }
        }

        /// <summary>
        /// Provider
        /// </summary>
        public virtual string ProviderName
        {
            get
            {
                if (ConnectionStringSettings == null)
                    throw new QueryException("ConnectionStringSettings is not set.");
                return ConnectionStringSettings.ProviderName;
            }
        }

        private ConnectionStringSettings _connectionStringSettings;
        /// <summary>
        /// 连接字符串设置
        /// </summary>
        public virtual ConnectionStringSettings ConnectionStringSettings { get { return _connectionStringSettings; } set { _connectionStringSettings = value; } }

        private string where;
        /// <summary>
        /// where clause
        /// </summary>
        public string WhereClause
        {
            get
            {
                if (where == null)
                {
                    StringBuilder sb = new StringBuilder();
                    AppendWhere(sb);
                    where = sb.ToString();
                }

                return where;
            }
            set { where = value; }
        }

        private string orderby = null;
        /// <summary>
        /// order by clause
        /// </summary>
        public string OrderByClause
        {
            get
            {
                if (orderby == null)
                    orderby = GetOrderBy();
                return orderby;
            }
            set { orderby = value; }
        }

        private string tableName;
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get
            {
                if (tableName == null)
                    tableName = GetTableName();
                return tableName;
            }
            set { tableName = value; }
        }

        private string tableField;
        /// <summary>
        /// 主键名称
        /// </summary>
        public string TableField
        {
            get
            {
                if (tableField == null)
                    tableField = GetTableField();

                return tableField;
            }
            set { tableField = value; }
        }

        private bool? appendWhereKeyword;

        /// <summary>
        /// if append "where" clause to sql 
        /// </summary>
        public bool AppendWhereKeyword
        {
            get
            {
                if (appendWhereKeyword == null)
                    appendWhereKeyword = GetAppendWhereKeyword();

                return appendWhereKeyword.Value;
            }
            set
            {
                appendWhereKeyword = value;
            }
        }

        #endregion

        /// <summary>
        /// set connection string name
        /// </summary>
        /// <param name="name"></param>
        public void SetConnectionStringName(string name)
        {
            _connectionStringSettings = ConfigBase.GetConnectionStringSettings(name);
        }

        #region virtual / abstract

        /// <summary>
        /// this method is called before query condition is translated to sql statement.
        /// </summary>
        public virtual void BeforeQuery()
        {
        }

        protected virtual string GetTableName() { return null; }
        protected virtual string GetTableField() { return "Id"; }
        protected virtual bool GetAppendWhereKeyword() { return true; }

        /// <summary>
        /// load query conditions(normally from querystring)
        /// </summary>
        public virtual void LoadCondidtion()
        {
        }

        protected virtual void AppendWhere(StringBuilder where) { }

        protected virtual string GetOrderBy() { return string.Empty; }

        #endregion

        #region ICachable Members

        /// <summary>
        /// 缓存key
        /// </summary>
        public virtual string CacheKey { get { return string.Format("{0}.query:{1}", GetType().Name, SecurityUtil.MD5_Hash(WhereClause)); } }

        /// <summary>
        /// 父级缓存key
        /// </summary>
        public virtual string ParentCacheKey { get; set; }

        /// <summary>
        /// 数目缓存key
        /// </summary>
        public virtual string CountCacheKey { get { return string.Format("{0}.count:{1}", GetType().Name, SecurityUtil.MD5_Hash(WhereClause)); } }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public virtual TimeSpan CacheTime
        {
            get
            {
                return CacheConfig.ValidFor;
            }
        }

        #endregion

        /// <summary>
        /// no paging query
        /// </summary>
        public QueryCondition NoPaging()
        {
            return NoPaging(0);
        }

        /// <summary>
        /// top N query
        /// </summary>
        /// <param name="topN"></param>
        public QueryCondition NoPaging(int topN)
        {
            PageSize = 0;
            TotalCount = topN;

            return this;
        }

        protected string GetClause(string column, bool? b)
        {
            if (b == null)
                return string.Empty;

            return string.Format("{0}={1}", column, b.Value ? 1 : 0);
        }

        protected string GetClause(string column, string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return string.Format("{0}='{1}'", column, str);
        }

        protected string GetClause(string column, int? i)
        {
            if (i == null)
                return string.Empty;

            return string.Format("{0}={1}", column, i.Value);
        }

        protected string GetClause(string column, int i)
        {
            if (i <= 0)
                return string.Empty;

            return string.Format("{0}={1}", column, i);
        }

        /// <summary>
        /// 获取记录Id列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetRelationIds<T>()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            if (!CacheConfig.Enabled)
                return provider.GetRelationIds<T>(this);

            string key = GetCacheKey();
            if (StringUtil.IsNullOrEmpty(key))
                return provider.GetRelationIds<T>(this);

            List<T> ids = JCache.Get<List<T>>(key);
            if (ids == null)
            {
                ids = provider.GetRelationIds<T>(this);

                if (ids != null)
                    JCache.Insert(key, ids, CacheTime);
            }

            return ids;
        }

        public List<int> GetRelationIds()
        {
            return GetRelationIds<int>();
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <returns></returns>
        public int GetRelationCount()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            if (!CacheConfig.Enabled)
                return provider.Count(this);

            string key = GetCountCacheKey();
            if (StringUtil.IsNullOrEmpty(key))
                return provider.Count(this);

            int? count = JCache.Get<int?>(key);
            if (count == null || !count.HasValue)
            {
                count = provider.Count(this);

                JCache.Insert(key, count, CacheTime);
            }

            return count.Value;
        }

        /// <summary>
        /// delete
        /// </summary>
        public void Delete()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            provider.Delete(this);
        }

        /// <summary>
        /// get IDataReader from querycondition, no cache
        /// </summary>
        /// <returns></returns>
        public IDataReader GetReader()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            return provider.GetReader(this);
        }

        /// <summary>
        /// get DataTable from query condition
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            using (IDataReader rdr = GetReader())
            {
                DataTable schema = rdr.GetSchemaTable();

                DataTable dt = new DataTable();

                foreach (DataRow row in schema.Rows)
                {
                    string columnName = row.ItemArray[0].ToString();
                    dt.Columns.Add(columnName);
                }

                while (rdr.Read())
                {
                    DataRow r = dt.NewRow();
                    foreach (DataColumn column in dt.Columns)
                    {
                        r[column] = rdr[column.ColumnName];
                    }
                    dt.Rows.Add(r);
                }

                return dt;
            }
        }

        public Hashtable GetHashtable()
        {
            using (IDataReader rdr = GetReader())
            {
                DataTable schema = rdr.GetSchemaTable();
                if (rdr.Read())
                {
                    return DataUtil.GetHashtable(rdr, schema);
                }

                return null;
            }
        }

        #region cache key

        /// <summary>
        /// 获取缓存Key
        /// </summary>
        /// <returns></returns>
        string GetCacheKey()
        {
            string key = StringUtil.GetSafeCacheKey(CacheKey);
            if (StringUtil.IsNullOrEmpty(key))
                return string.Empty;

            if (StringUtil.HasText(ParentCacheKey))
                AddToParentCacheKey(StringUtil.GetSafeCacheKey(ParentCacheKey), key, CacheTime);

            return key;
        }

        /// <summary>
        /// 获取总数的缓存key
        /// </summary>
        /// <returns></returns>
        string GetCountCacheKey()
        {
            string key = StringUtil.GetSafeCacheKey(CountCacheKey);
            if (StringUtil.IsNullOrEmpty(key))
                return string.Empty;

            if (StringUtil.HasText(ParentCacheKey))
                AddToParentCacheKey(StringUtil.GetSafeCacheKey(ParentCacheKey), key, CacheTime);

            return key;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        void RemoveCache()
        {
            if (!StringUtil.HasText(ParentCacheKey))
            {
                JCache.Remove(StringUtil.GetSafeCacheKey(CountCacheKey));
                JCache.Remove(StringUtil.GetSafeCacheKey(CacheKey));
            }
            else
            {
                string cacheKey = StringUtil.GetSafeCacheKey(ParentCacheKey);
                List<string> keys = JCache.Get<List<string>>(cacheKey) ?? new List<string>();
                foreach (string key in keys)
                {
                    JCache.Remove(key);
                }
                JCache.Remove(cacheKey);
            }
        }

        void AddToParentCacheKey(string parentCacheKey, string cacheKey, TimeSpan cacheTime)
        {
            List<string> cachekeys = JCache.Get<List<string>>(parentCacheKey) ?? new List<string>();

            if (!cachekeys.Contains(cacheKey))
                cachekeys.Add(cacheKey);

            JCache.Insert(parentCacheKey, cachekeys, cacheTime);
        }

        #endregion

        public class Conditions
        {
            string logic_oper = "AND";

            public Conditions()
            {

            }

            public Conditions(string logic_oper)
            {
                this.logic_oper = logic_oper;
            }

            List<string> clauses = new List<string>();

            public void Add(string columnName, int? value)
            {
                Add(columnName, value, value == null);
            }

            public void Add(string columnName, int value)
            {
                Add(columnName, value, value <= 0);
            }

            public void Add(string columnName, object value)
            {
                Add(columnName, value, value == null);
            }

            public void Add(string columnName, object value, bool igore)
            {
                Add(columnName, value, igore, "=");
            }

            public void Add(string columnName, object value, bool ignore, string oper)
            {
                if (!ignore)
                {
                    string clause = string.Empty;
                    if (value == null)
                        clause = string.Format("{0} is null", columnName);
                    else
                    {
                        if (value is bool)
                            clause = string.Format("{0}{1}{2}", columnName, oper, (bool)value ? 1 : 0);
                        else
                            clause = string.Format("{0}{1}'{2}'", columnName, oper, value);
                    }

                    if (!string.IsNullOrEmpty(clause))
                        clauses.Add(clause);
                }
            }

            public override string ToString()
            {
                return StringUtil.CollectionToDelimitedString(clauses, string.Format(" {0} ", logic_oper), string.Empty);
            }
        }
    }
}
