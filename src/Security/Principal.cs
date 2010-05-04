using System.Security.Principal;

namespace Kiss.Security
{
    public class Principal : IPrincipal
    {
        private IIdentity _user;

        public Principal(IIdentity user)
        {
            _user = user;
        }

        public bool HasPermission(string permission)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().HasPermission(_user, permission);
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _user; }
        }

        public bool IsInRole(string role)
        {
            return ServiceLocator.Instance.Resolve<IUserService>().IsInRole(_user, role);
        }

        #endregion
    }
}
