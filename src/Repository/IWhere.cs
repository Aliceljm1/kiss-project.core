using System.Data;

namespace Kiss
{
    public interface IWhere
    {
        int Count();
        T Select<T>(string field);
        DataTable Select(params string[] fields);
        void Delete();
        void Update();
        IWhere Set(string column, string value);
        IWhere Where(string where, params object[] args);
    }
}
