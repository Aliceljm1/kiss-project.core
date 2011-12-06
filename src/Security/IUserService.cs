using System.Collections.Generic;
using System.Security.Principal;

namespace Kiss.Security
{
    /// <summary>
    /// 用户服务的接口
    /// </summary>
    public interface IUserService
    {
        #region permission

        IModule GetModuleByName(string moduleName);

        bool HasPermission(IIdentity identity, string permission);
        List<bool> HasPermission(IIdentity identity, string[] permissions);
        void AddPermissionModule(string module_name, string title, string action);
        void RemovePermissionModule(string module_name, string action_name);

        IPermission GetPermissionByPermissionId(string id);
        IPermission[] GetsPermissionByPermissionIds(string[] ids);

        void RemovePermission(string moduleId, string instance);
        void RemovePermission(string instance);
        void RemovePermissionByPermissionId(string permissionId);

        void AddPermission(params IPermission[] permissions);
        IPermission NewPermission(string moduleId, string instance, int resType, string resId, long flag, int level);
        IPermission[] GetsPermissionByInstance(string moduleId, string instance);
        IPermission GetPermissionByResId(string moduleId, int resType, string resId);

        #endregion

        #region Role

        IRole[] GetsAllRole();
        IRole GetRoleByRoleId(string roleId);
        bool IsInRole(IIdentity identity, string role);

        #endregion

        #region User

        IUser GetUserByUserId(string userid);
        IUser[] GetsUserByUserIds(string[] userIds);

        IUser GetUserByUserName(int siteId, string username);

        IUser GetUserInfo(IIdentity identity);
        void SaveUserInfo(params IUser[] users);

        IUser[] GetsUserByRoleId(string roleId);
        IUser[] GetsUserByDeptId(string deptId);
        IUser[] GetsUserByGroupId(string groupId);

        #endregion

        #region Dept

        IDept GetDeptByDeptId(string deptId);
        IDept[] GetsDeptByDeptIds(string[] deptIds);

        #endregion
    }
}
