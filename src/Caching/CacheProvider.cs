using System;
using System.Collections.Generic;

namespace Kiss.Caching
{
    /// <summary>
    /// abstract cache Provider
    /// </summary>
    public interface ICacheProvider
    {
        void Insert(string key, object obj, TimeSpan validFor);

        object Get(string key);

        IDictionary<string, object> Get(IEnumerable<string> keys);

        void Remove(string key);
    }
}
