using Kiss.Utils;
using System.Collections.Generic;

namespace Kiss.Caching
{
    class SimpleHierarchyCachePlugin
    {
        public void Start()
        {
            QueryObject.Saved += Obj_Saved;
            QueryObject.Batch += Obj_Batch;

            QueryObject.PreQuery += Obj_PreQuery;
            QueryObject.AfterQuery += Obj_AfterQuery;
        }

        public void Stop()
        {
            QueryObject.Saved -= Obj_Saved;
            QueryObject.Batch -= Obj_Batch;

            QueryObject.PreQuery -= Obj_PreQuery;
            QueryObject.AfterQuery -= Obj_AfterQuery;
        }

        void Obj_Saved(object sender, SavedEventArgs e)
        {
            if (e.Action == SaveAction.None)
                return;

            string typefullname = sender.GetType().FullName;

            string key = null;

            if (sender is Kiss.Obj<int>)
                key = (sender as Kiss.Obj<int>).Id.ToString();
            else if (sender is Kiss.Obj<string>)
                key = (sender as Kiss.Obj<string>).Id;

            // update/add self cache
            JCache.Insert(typefullname + ".obj:" + key ?? sender.ToString(), sender);           
        }

        void Obj_Batch(object sender, BatchEventArgs e)
        {
            JCache.RemoveHierarchyCache(JCache.GetRootCacheKey(e.Type.FullName));
        }

        void Obj_PreQuery(object sender, QueryObject.QueryEventArgs e)
        {
            e.Result = JCache.Get(e.Type.FullName + ".query:" + SecurityUtil.MD5_Hash(e.Sql));
        }

        void Obj_AfterQuery(object sender, QueryObject.QueryEventArgs e)
        {
            if (e.Result == null)
                return;

            string typefullname = e.Type.FullName;

            string key = typefullname + ".query:" + SecurityUtil.MD5_Hash(e.Sql);

            string root_key = JCache.GetRootCacheKey(typefullname);

            List<string> sub_keys = JCache.Get<List<string>>(root_key) ?? new List<string>();
            if (!sub_keys.Contains(key))
                sub_keys.Add(key);

            JCache.Insert(root_key, sub_keys);
            JCache.Insert(key, e.Result);
        }
    }
}
