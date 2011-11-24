using System;
using System.Collections.Generic;
using Kiss.Security;

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

        public static INoticeConfig Config
        {
            get
            {
                return ServiceLocator.Instance.Resolve("kiss.noticeconfig") as INoticeConfig;
            }
        }

        public static void Send(string msgType, string title, string content, IUser from, params IUser[] to)
        {
            if (to.Length == 0) return;

            foreach (var item in Config.GetsValidChannel(msgType, to))
            {
                Create(item.Key).Send(title, content, from, item.Value);
            }
        }

        public static void Send(string msgType, string templateId, Dictionary<string, object> param, IUser from, params IUser[] to)
        {
            if (to.Length == 0) return;

            foreach (var item in Config.GetsValidChannel(msgType, to))
            {
                Create(item.Key).Send(templateId, param, from, item.Value);
            }
        }
    }
}
