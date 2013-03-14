using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using Kiss.Query;

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

        void RemovePermission(string moduleId, string[] instances);
        void RemovePermission(params string[] instances);
        void RemovePermissionByPermissionId(string permissionId);

        void AddPermission(params IPermission[] permissions);
        IPermission NewPermission(string moduleId, string instance, int resType, string resId, long flag, int level);
        IPermission[] GetsPermissionByInstance(string moduleId, string instance);
        IPermission GetPermissionByResId(string moduleId, int resType, string resId);
        IPermission[] GetsPermissionByInstance(IUser user, string moduleId, string instance);

        #endregion

        #region Role

        IRole[] GetsAllRole();
        IRole GetRoleByRoleId(string roleId);
        IRole[] GetsRoleByUserId(string userId);
        bool IsInRole(IIdentity identity, string role);
        void UpdateUserRoles(string userid, string[] roleIds);

        #endregion

        #region User

        IUser GetUserByUserId(string userid);
        IUser[] GetsUserByUserIds(string[] userIds);

        IUser GetUserByUserName(string username);

        IUser GetUserInfo(IIdentity identity);
        IUser NewUserInfo(string username);
        void SaveUserInfo(params IUser[] users);

        bool DeleteUser(params string[] userIds);

        IUser[] GetsUserByRoleId(string roleId);
        IUser[] GetsUserByDeptId(string deptId);
        IUser[] GetsUserByGroupId(string groupId);

        DataTable QueryUser(QueryCondition qc);

        bool SaveUserAvator(string userId, byte[] content);

        bool Authenticate(string username, string password);

        #endregion

        #region Dept

        IDept[] GetsDeptByUserId(string userId);
        IDept GetDeptByDeptId(string deptId);
        IDept[] GetsDeptByDeptIds(string[] deptIds);

        #endregion

        #region Group
        IGroup NewGroup(string title);
        void SaveGroup(IGroup group);
        void AddUser2Group(string groupId, string[] userids, int usertype);
        void RemoveUserFromGroup(string groupId, string[] userids);
        bool IsInGroup(IIdentity identity, string siteId);
        IGroup GetGroupByGroupId(string groupId);
        IGroup[] GetsGroupByGroupIds(string[] groupIds);
        IGroup[] GetsGroupByUserId(string userid);
        #endregion

        #region Site
        ISite NewSite(string authority);
        void SaveSite(ISite site);
        ISite[] GetsSiteByUserId(string userid);
        ISite GetSiteBySiteId(string siteId);
        ISite[] GetsSiteBySiteIds(string[] siteIds);
        void AddUser2Site(string siteId, string[] userids, int usertype);
        void RemoveUserFromSite(string siteId, string[] userids);
        bool IsInSite(IIdentity identity, string siteId);
        #endregion
    }
}
