
namespace Kiss
{
    public interface IWhere
    {
        int Count();
        void Delete();
        void Update();
        IWhere Set(string column, string value);
        IWhere Where(string where, params object[] args);
    }
}
