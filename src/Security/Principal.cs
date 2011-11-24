using System;
using System.Security.Principal;
using System.Collections.Generic;

namespace Kiss.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class Principal : IPrincipal
    {
        private IIdentity _user;

        public Principal(IIdentity user)
        {
            _user = user;
        }

        /// <summary>
        /// 检查当前用户是否有指定的权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(string permission)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().HasPermission(_user, permission);
        }

        /// <summary>
        /// 检查当前用户是否有指定的权限，如果没有权限，则抛出权限拒绝事件
        /// </summary>
        /// <param name="permission"></param>
        public void CheckPermission(string permission)
        {
            if (!HasPermission(permission))
                OnPermissionDenied(new PermissionDeniedEventArgs(permission));
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _user; }
        }

        public bool IsInRole(string role)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().IsInRole(Identity, role);
        }

        #endregion

        public IUser Info
        {
            get
            {
                return ServiceLocator.Instance.Resolve<IUserService>().GetUserInfo(Identity);
            }
        }

        #region events

        public static event EventHandler<PermissionDeniedEventArgs> PermissionDenied;

        public void OnPermissionDenied(PermissionDeniedEventArgs e)
        {
            EventHandler<PermissionDeniedEventArgs> handler = PermissionDenied;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }

    public interface IUser
    {
        string Id { get; }
        string UserName { get; }
        string DisplayName { get; set; }
        string Email { get; set; }
        string Password { get; }

        string[] GetsRoleId();
        string[] GetsDeptId();
        string[] GetsDeptId(bool inherit);

        string this[string prop] { get; set; }

        int GetPermissionLevel(string moduleId, string instance, string action);

        ExtendedAttributes ExtAttrs { get; }
    }

    public interface IPermission
    {
        string Id { get; }

        string ModuleId { get; set; }

        string Instance { get; set; }
        long Flag { get; set; }

        string ResId { get; set; }
        int ResType { get; set; }

        /// <summary>
        /// 权限继承级别
        /// </summary>
        int Level { get; set; }
    }

    public interface IModule
    {
        string Id { get; }
        string Name { get; }
        string Title { get; }
        string PermissionTitles { get; }
        string PermissionNames { get; }
        string PermissionFlags { get; }

        List<string> Titles { get; }
        List<string> Names { get; }
        List<string> Flags { get; }

        int TotalFlag { get; }
    }

    public interface IRole
    {
        string Id { get; }
        string Name { get; }
        string Title { get; }
    }

    public interface IDept
    {
        string Id { get; }
        string Title { get; }
        string ParentId { get; }
    }

    /// <summary>
    /// PermissionDenied EventArgs
    /// </summary>
    public class PermissionDeniedEventArgs : EventArgs
    {
        public static readonly new PermissionDeniedEventArgs Empty = new PermissionDeniedEventArgs();

        public string Permission { get; private set; }

        public PermissionDeniedEventArgs()
        {
        }

        public PermissionDeniedEventArgs(string permission)
        {
            Permission = permission;
        }
    }
}
