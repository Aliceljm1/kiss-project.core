using System;
using System.Xml;
using Kiss.Config;

namespace Kiss.Caching
{
    /// <summary>
    /// cache config
    /// </summary>
    [ConfigNode("caching", Desc = "cache")]
    public class CacheConfig : ConfigWithProviders
    {
        /// <summary>
        /// get cache config instance
        /// </summary>
        public static CacheConfig Instance
        {
            get { return GetConfig<CacheConfig>(); }
        }

        /// <summary>
        /// cache valid days
        /// </summary>
        [ConfigProp("cacheDay", ConfigPropAttribute.DataType.Int, DefaultValue = 1, Desc = "cache valid days")]
        public int CacheDay { get; private set; }

        [ConfigProp("enabled", ConfigPropAttribute.DataType.Boolean, DefaultValue = true)]
        public bool Enabled { get; private set; }

        /// <summary>
        /// cache key's namespace
        /// </summary>
        [ConfigProp("namespace", DefaultValue = "kiss")]
        public string Namespace { get; private set; }

        /// <summary>
        /// cache valid times
        /// </summary>
        public TimeSpan ValidFor { get; private set; }

        protected override void LoadConfigsFromConfigurationXml(XmlNode node)
        {
            base.LoadConfigsFromConfigurationXml(node);

            if (CacheDay < 0 || CacheDay >= 30)
                CacheDay = 1;

            ValidFor = TimeSpan.FromDays(CacheDay);
        }
    }
}
