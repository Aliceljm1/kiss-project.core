using System;
using System.Security.Principal;

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

        string this[string prop] { get; set; }

        ExtendedAttributes ExtAttrs { get; }
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
