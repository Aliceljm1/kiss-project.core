using System;
using System.Collections.Generic;
using System.Text;
using Gala.Utils;
using System.Globalization;
using System.Resources;

namespace Gala
{
    public class ResourceBundle
    {
        public string LocaleRoot = null;

        /// <summary>
        /// ResourceBundle is singleton
        /// </summary>
        /// <returns></returns>
        public static ResourceBundle Instance
        {
            get
            {
                if ( Singleton<ResourceBundle>.Instance == null )
                    Singleton<ResourceBundle>.Instance = new ResourceBundle ( );

                return Singleton<ResourceBundle>.Instance;
            }
        }

        /// <summary>
        /// singleton class, GetInstance() is the only place 
        /// that will instantiate a ResourceBundle
        /// </summary>
        private ResourceBundle ( )
        {
            _rmDictionary = new Dictionary<string, ResourceManager> ( );
            _rmFiles = new List<string> ( );

            _locale = SpecificCultures.EN_US;   //default
        }

        /// <summary>
        /// Set Locale for the application, such as "en-US"
        /// </summary>
        public string Locale
        {
            get { return _locale; }
            set
            {
                _locale = value;
                _culture = new CultureInfo ( _locale );

                UpdateResourceManagers ( );
            }
        }

        /// <summary>
        /// Bundle a .resx file in sub locale folder
        /// The file will be used to retrieve resource later according to 
        /// the locale used by the application
        /// </summary>
        /// <param name="name"></param>
        public void AddResourceFile ( string filename )
        {
            _rmDictionary.Add ( filename, null );
            _rmFiles.Add ( filename );
        }

        public string GetString ( string filename, string resourcename )
        {
            ResourceManager rm = _rmDictionary[ filename ];
            if ( rm == null )
                throw new Exception ( filename + " was not found or was not added to ResourceBundle" );

            return rm.GetString ( resourcename );
        }

        public string GetString ( string filename, string resourcename, string defaultStr )
        {
            string str = GetString ( filename, resourcename );
            if ( String.IsNullOrEmpty ( str ) )
            {
                str = defaultStr;
            }
            return str;
        }

        public Object GetObject ( string filename, string resourcename )
        {
            ResourceManager rm = _rmDictionary[ filename ];
            if ( rm == null )
                throw new Exception ( filename + " was not found or was not added to ResourceBundle" );

            return rm.GetObject ( resourcename );
        }

        private void UpdateResourceManagers ( )
        {
            string localeDir = _locale.Replace ( '-', '_' );

            if ( LocaleRoot == null )
                throw new Exception ( "LocaleRoot needs to be set before Locale is set." );

            if ( _rmDictionary.Count == 0 )
                throw new Exception ( "AddResourceFile() needs to be called before Locale is set." );

            string basename = LocaleRoot + "." + localeDir;
            foreach ( string key in _rmFiles )
            {//key is filename
                string type = basename + "." + key;
                //set a new ResourceManager to match current locale
                ResourceManager rm = _rmDictionary[ key ];
                if ( rm != null )
                    rm.ReleaseAllResources ( );
                _rmDictionary[ key ] = null;
                rm = new ResourceManager ( type, Assembly.GetExecutingAssembly ( ) );
                _rmDictionary[ key ] = rm;
            }
        }

        private string _locale;
        private CultureInfo _culture;

        private Dictionary<string, ResourceManager> _rmDictionary;
        private List<String> _rmFiles;

        private static ResourceBundle _instance;
    }
}
