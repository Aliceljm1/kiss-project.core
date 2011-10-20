using Kiss.Security;

namespace Kiss.Notice
{
    public interface INotice
    {
        void Send(IUser from, IUser to, string title, string content);
    }    
}
