using System.Configuration;
using System.Xml;

namespace Kiss.Config
{
    /// <summary>
    /// 定义了自定义配置的入口
    /// </summary>
    internal class ConfigHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        /// <summary>
        /// 创建配置section handler
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public object Create ( object parent, object configContext, XmlNode section )
        {
            Configuration config = new Configuration ( );
            config.LoadValuesFromConfigurationXml ( section );

            return config;
        }

        #endregion
    }
}
