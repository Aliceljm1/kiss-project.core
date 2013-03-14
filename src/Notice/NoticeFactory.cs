using Kiss.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        /// <summary>
        /// 根据模板Id 和 doc 获得标题和内容
        /// </summary>
        public static bool ResolverTemplate(string channel, string templateId, Dictionary<string, object> param, out string title, out string content)
        {
            title = content = string.Empty;

            DictSchema schema = (from q in DictSchema.CreateContext(true)
                                 where q.Type == "msg" && q.Name == "template" && q.Title == templateId
                                 select q).FirstOrDefault();

            if (schema == null)
            {
                LogManager.GetLogger<NoticeFactory>().ErrorFormat("指定的模板：{0} 不存在。", templateId);
                return false;
            }

            ITemplateEngine te = ServiceLocator.Instance.Resolve<ITemplateEngine>();

            using (StringWriter sw = new StringWriter())
            {
                string template = schema[channel + "_title"];
                if (string.IsNullOrEmpty(template))
                    template = schema["default_title"];

                te.Process(param, string.Empty, sw, template);

                title = sw.GetStringBuilder().ToString();
            }

            using (StringWriter sw = new StringWriter())
            {
                string template = schema[channel + "_content"];
                if (string.IsNullOrEmpty(template))
                    template = schema["default_content"];

                te.Process(param, string.Empty, sw, template);

                content = sw.GetStringBuilder().ToString();
            }

            return true;
        }
    }
}
