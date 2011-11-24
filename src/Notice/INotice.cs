using System.Collections.Generic;
using Kiss.Security;

namespace Kiss.Notice
{
    /// <summary>
    /// notice interface
    /// </summary>
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

    /// <summary>
    /// notice config interface
    /// </summary>
    public interface INoticeConfig
    {
        /// <summary>
        /// get specified user's valid channel of specified msg type
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        Dictionary<string, IUser[]> GetsValidChannel(string msgType, IEnumerable<IUser> users);
    }
}
