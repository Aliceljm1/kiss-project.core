using System;
using System.Collections.Generic;
using System.Web;

namespace Kiss.Plugin
{
    public class PluginBootstrapper
    {
        internal Dictionary<string, PluginSetting> _pluginSettings = new Dictionary<string, PluginSetting>();

        /// <summary>Gets plugins in the current app domain using the type finder.</summary>
        /// <returns>An enumeration of available plugins.</returns>
        public IEnumerable<IPluginDefinition> GetPluginDefinitions()
        {
            List<IPluginDefinition> pluginDefinitions = new List<IPluginDefinition>();

            // autoinitialize plugins
            foreach (Type type in ServiceLocator.Instance.Resolve<ITypeFinder>().Find(typeof(IPluginInitializer)))
            {
                foreach (AutoInitAttribute plugin in type.GetCustomAttributes(typeof(AutoInitAttribute), true))
                {
                    plugin.InitializerType = type;

                    pluginDefinitions.Add(plugin);
                }
            }

            pluginDefinitions.Sort();
            pluginDefinitions.Reverse();
            return pluginDefinitions;
        }

        /// <summary>Invokes the initialize method on the supplied plugins.</summary>
        public void InitializePlugins(IEnumerable<IPluginDefinition> plugins)
        {
            ServiceLocator sl = ServiceLocator.Instance;

            PluginSettings settings = PluginConfig.Instance.Settings;

            List<Exception> exceptions = new List<Exception>();
            int count = 0, enable_count = 0;
            foreach (IPluginDefinition plugin in plugins)
            {
                try
                {
                    PluginSetting setting = settings.FindByName(plugin.Name);
                    setting.Title = plugin.Title;
                    setting.Description = plugin.Description;

                    _pluginSettings[plugin.Name] = setting;

                    plugin.Init(sl, setting);
                    count++;
                    if (setting.Enable)
                        enable_count++;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                string message = "While initializing {0} plugin(s) threw an exception. Please review the stack trace to find out what went wrong.";
                message = string.Format(message, exceptions.Count);

                foreach (Exception ex in exceptions)
                    message += Environment.NewLine + Environment.NewLine + "- " + ex.Message;

                throw new PluginInitException(message, exceptions.ToArray());
            }

            LogManager.GetLogger<PluginBootstrapper>().Info("plugins initialized. {1} of {0} is enable.", count, enable_count);
        }
    }
}
