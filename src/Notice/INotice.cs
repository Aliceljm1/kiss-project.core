using System.Collections.Generic;
using Kiss.Security;

namespace Kiss.Notice
{
    public interface INotice
    {
        /// <summary>
        /// send notify
        /// </summary>
        void Send(string title, string content, IUser from, params IUser[] to);

        /// <summary>
        /// send notify using template
        /// </summary>
        void Send(string templateId, Dictionary<string, object> param, IUser from, params IUser[] to);
    }
}
