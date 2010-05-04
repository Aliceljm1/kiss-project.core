#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    ProviderHelper.cs
//+ File Created:   20090729
//+-------------------------------------------------------------------+
//+ Purpose:        用于创建Provider
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Configuration.Provider;
using System.Web;
using Kiss.Config;

namespace Kiss
{
    /// <summary>
    /// 使用该类创建Provider
    /// </summary>
    public static class ProviderHelper
    {

        /// <summary>
        /// 创建Provider
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ProviderBase CreateProvider(ConfigWithProviders config)
        {
            string cacheKey = config.ConfigName + "::" + config.DefaultProvider;

            ProviderBase objProvider = HttpRuntime.Cache.Get(cacheKey) as ProviderBase;
            if (objProvider == null)
            {
                try
                {
                    // Read the configuration specific information for this provider
                    ConfigWithProviders.Provider provider = config.Providers[config.DefaultProvider];

                    // The assembly should be in \bin or GAC
                    Type type = Type.GetType(provider.Type);
                    objProvider = (ProviderBase)Activator.CreateInstance(type);

                    objProvider.Initialize(provider.Name, provider.Attributes);

                    // 添加到缓存
                    if (objProvider != null)
                        HttpRuntime.Cache.Insert(cacheKey, objProvider);

                }
                catch (Exception e)
                {
                    throw new KissException(Resource.LoadProviderFailed, e);
                }
            }

            return objProvider;
        }
    }
}
