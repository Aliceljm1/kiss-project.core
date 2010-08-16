using System;
using System.Linq;
using System.Collections.Generic;

namespace Kiss
{
    /// <summary>
    /// base model class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class Obj<t> : QueryObject
    {
        #region props

        /// <summary>
        /// Id
        /// </summary>
        public virtual t Id { get; set; }

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
        public static bool operator ==(Obj<t> first, Obj<t> second)
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
        public static bool operator !=(Obj<t> first, Obj<t> second)
        {
            return !(first == second);
        }

        #endregion
    }

    /// <summary>
    /// dict schema
    /// </summary>
    [Serializable]
    [OriginalName("gDictSchema")]
    public class DictSchema : QueryObject<DictSchema, int>, IComparable<DictSchema>
    {
        [PK]
        public override int Id { get { return base.Id; } set { base.Id = value; } }

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

        #region Api

        public static DictSchema GetRoot(int siteId, string type)
        {
            return (from q in Query
                    where q.SiteId == siteId && q.Type == type && q.Depth == 0
                    select q).SingleOrDefault();
        }

        public static void DeleteByRoot(int siteId, string type)
        {
            List<DictSchema> list = (from q in Query
                                     where q.SiteId == siteId && q.Type == type && q.Depth > 0
                                     select q).ToList();

            foreach (var schema in list)
            {
                Query.Remove(schema);
            }

            Query.SubmitChanges(true);
        }

        public static void Copy(int fromSiteId, int toSiteId)
        {
            List<DictSchema> oldList = GetsBySiteId(fromSiteId);
            foreach (DictSchema d0 in oldList)
            {
                d0.Id = 0;
                d0.SiteId = toSiteId;
                Query.Add(d0);
                Query.SubmitChanges();

                List<DictSchema> list = GetsByType(fromSiteId, d0.Type);

                foreach (DictSchema d1 in list)
                {
                    d1.ParentId = d0.Id;
                    copyRecu(d1, toSiteId);
                }
            }
        }

        private static void copyRecu(DictSchema schema, int tositeId)
        {
            schema.Id = 0;
            schema.SiteId = tositeId;

            Query.Add(schema);
            Query.SubmitChanges();

            if (schema.HasChild && schema.Children != null && schema.Children.Count > 0)
            {
                foreach (DictSchema s in schema.Children)
                {
                    s.ParentId = schema.Id;
                    copyRecu(s, tositeId);
                }
            }
        }

        public static List<DictSchema> GetsBySiteId(int siteId)
        {
            return (from q in Query
                    where q.SiteId == siteId && q.ParentId == 0
                    select q).ToList();
        }

        public static DictSchema GetByName(int siteId, string type, string name)
        {
            return (from q in Query
                    where q.SiteId == siteId && q.Type == type && q.Name == name
                    select q).FirstOrDefault();
        }

        public static List<DictSchema> GetsByType(int siteId, string type)
        {
            List<DictSchema> list = (from q in Query
                                     where q.SiteId == siteId && q.Type == type && q.Depth > 0
                                     orderby q.Depth ascending, q.SortOrder ascending
                                     select q).ToList();

            // re group            
            List<DictSchema> result = list.FindAll(delegate(DictSchema s)
            {
                return s.Depth == 1;
            });

            foreach (DictSchema s in result)
            {
                regroup(s, list);
            }

            return result;
        }

        private static void regroup(DictSchema s, List<DictSchema> list)
        {
            if (s.HasChild)
            {
                s.Children = list.FindAll(delegate(DictSchema schema)
                {
                    return schema.ParentId == s.Id;
                });

                foreach (DictSchema schema in s.Children)
                {
                    schema.Parent = s;
                    regroup(schema, list);
                }
            }
        }

        public static List<DictSchema> GetsByParentId(int id)
        {
            return (from q in Query
                    where q.ParentId == id
                    orderby q.SortOrder ascending
                    select q).ToList();
        }

        //protected override void OnDelete(params DictSchema[] objs)
        //{
        //    base.OnDelete(objs);

        //    foreach (DictSchema s in objs)
        //    {
        //        delRecu(s);
        //    }
        //}

        private static void delRecu(DictSchema s)
        {
            if (!s.HasChild)
                return;

            foreach (DictSchema sub in GetsByParentId(s.Id))
            {
                Query.Remove(sub);
                Query.SubmitChanges();
            }
        }

        #endregion

        #region event

        public class FilterEventArgs : EventArgs
        {
            public static readonly new FilterEventArgs Empty = new FilterEventArgs();

            public List<DictSchema> SchemaList { get; set; }

            public string Type { get; set; }
        }

        public static event EventHandler<FilterEventArgs> Filter;

        public static void OnFilter(string type, List<DictSchema> list)
        {
            FilterEventArgs e = new FilterEventArgs();
            e.SchemaList = list;
            e.Type = type;

            EventHandler<FilterEventArgs> handler = Filter;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        #endregion
    }
}
