using System.Security.Principal;

namespace Kiss.Security
{
    public interface IUserService
    {
        bool IsInRole(IIdentity identity, string role);
        bool HasPermission(IIdentity identity, string permission);
        void AddPermissionModule(string module_name, string title, string action);
        void RemovePermissionModule(string module_name, string action_name);
    }
}
