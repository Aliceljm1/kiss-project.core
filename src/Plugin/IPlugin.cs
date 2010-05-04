
using System;
using Kiss.Security;

namespace Kiss.Plugin
{
    public interface IPlugin : IComparable<IPlugin>
    {
        string Name { get; set; }
        Type Decorates { get; set; }
        int SortOrder { get; }
        bool IsAuthorized(Principal user);
        bool IsEnabled { get; }
    }
}
