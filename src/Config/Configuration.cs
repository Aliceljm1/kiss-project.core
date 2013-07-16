using Kiss.Utils;
using Kiss.XmlTransform;
using System;
using System.IO;
using System.Web;
using System.Xml;

namespace Kiss.Config
{
    /// <summary>
    /// 自定义配置
    /// </summary>
    internal class Configuration
    {
        #region fields

        private XmlNode root;

        private XmlNode _EmptyNode;

        private static readonly object _synclocker = new object();

        #endregion

        #region ctor

        private Configuration()
        {
        }

        #endregion

        #region methods

        public static Configuration GetConfig()
        {
            Configuration config = Singleton<Configuration>.Instance;

            if (config == null)
            {
                lock (_synclocker)
                {
                    config = Singleton<Configuration>.Instance;

                    if (config == null)
                    {
                        config = new Configuration();
                        config.Init();

                        Singleton<Configuration>.Instance = config;
                    }
                }
            }

            return config;
        }

        private void Init()
        {
            // 合并kiss.local.config和kiss.config
            string rootpath = AppDomain.CurrentDomain.BaseDirectory;

            if (HttpContext.Current != null)
                rootpath = Path.Combine(rootpath, "App_Data");

            using (XmlTransformableDocument x = new XmlTransformableDocument())
            {
                x.Load(Path.Combine(rootpath, "kiss.config"));

                string localfile = FileUtil.FormatDirectory(XmlUtil.GetStringAttribute(x.DocumentElement, "local", HttpContext.Current == null ? ".kiss.local.config" : Path.Combine(".App_Data", "kiss.local.config")));

                if (File.Exists(localfile))
                {
                    using (XmlTransformation t = new XmlTransformation(localfile))
                    {
                        t.Apply(x);
                    }
                }

                root = x.DocumentElement;

                _EmptyNode = x.CreateElement("__empty__");
            }
        }

        public XmlNode GetSection(string nodePath)
        {
            return root.SelectSingleNode(nodePath) ?? _EmptyNode;
        }

        #endregion
    }
}
