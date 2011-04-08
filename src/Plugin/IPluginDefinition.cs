
using System;

namespace Kiss.Plugin
{
    public interface IPluginDefinition : IComparable<IPluginDefinition>
    {
        void Init(ServiceLocator sl, ref PluginSetting setting);
        string Name { get; }
        string Title { get; set; }
        string Description { get; set; }
        int Priority { get; }
    }
}
