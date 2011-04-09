using Kiss.Plugin;

namespace Kiss.Caching
{
    [AutoInit(Priority = 5, Title = "HierarchyCache")]
    class HierarchyCacheInitializer : IPluginInitializer
    {
        public void Init(ServiceLocator sl, ref PluginSetting setting)
        {
            if (!setting.Enable) return;

            var module = new SimpleHierarchyCachePlugin();
            module.Start();
            sl.AddComponentInstance(module);
        }
    }
}
