using System;
using Kiss.Plugin;

namespace Kiss.Logging
{
    [AutoInit(Title = "Log", Priority = 99)]
    public class LogInitializer : IPluginInitializer
    {
        public void Init(ServiceLocator sl, PluginSetting setting)
        {
            if (!setting.Enable) return;

            string type = setting["type"] ?? string.Empty;
            if (string.IsNullOrEmpty(type))
                return;

            sl.AddComponent("kiss.logger", typeof(ILoggerFactory), Type.GetType(type, true, true));
        }
    }
}
