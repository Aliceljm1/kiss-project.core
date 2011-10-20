using System;

namespace Kiss.Notice
{
    public class NoticeFactory
    {
        public static INotice Create(string channel)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentException("channel name is empty.");
                
            return ServiceLocator.Instance.Resolve("kiss.notice." + channel) as INotice;
        }
    }
}
