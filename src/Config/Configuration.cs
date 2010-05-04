using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Caching;
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

        internal XmlNode EmptyNode;

        private static Dictionary<string,CacheDependency> CacheDependencyDict = new Dictionary<string, CacheDependency> ( );

        public static CacheDependency GetCacheDependency ( string key )
        {
            if ( CacheDependencyDict.ContainsKey ( key ) )
            {
                CacheDependency cd = CacheDependencyDict[ key ];
                if ( cd != null )
                    cd.Dispose ( );
            }

            CacheDependency cd2 = new CacheDependency ( Directory.GetFiles ( AppDomain.CurrentDomain.BaseDirectory, "*.config" ) );
            CacheDependencyDict[ key ] = cd2;

            return cd2;
        }

        private static bool ConfigChanged
        {
            get
            {
                foreach ( CacheDependency cd in CacheDependencyDict.Values )
                {
                    if ( cd.HasChanged )
                        return true;
                }

                return false;
            }
        }

        private const string NODENAME = "kiss";

        #endregion

        #region ctor

        internal Configuration ( )
        {
        }

        #endregion

        #region methods

        public static Configuration GetConfig ( )
        {
            // only refresh when httpcontext is null 
            if ( HttpContext.Current == null && ConfigChanged )
                ConfigurationManager.RefreshSection ( NODENAME );

            Configuration config = ConfigurationManager.GetSection ( NODENAME ) as Configuration;

            if ( config == null )
            {
                // get from dummy data
                if ( Singleton<Configuration>.Instance == null )
                {
                    config = new Configuration ( );
                    XmlDocument doc = new XmlDocument ( );
                    config.LoadValuesFromConfigurationXml ( doc.CreateElement ( "__dummyRoot__" ) );

                    Singleton<Configuration>.Instance = config;
                }

                config = Singleton<Configuration>.Instance;
            }

            return config;
        }

        public void LoadValuesFromConfigurationXml ( XmlNode node )
        {
            root = node;

            EmptyNode = node.OwnerDocument.CreateElement ( "__empty__" );
        }

        public XmlNode GetSection ( string nodePath )
        {
            return root.SelectSingleNode ( nodePath );
        }

        #endregion
    }
}
