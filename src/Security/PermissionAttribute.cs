using System;

namespace Kiss.Security
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PermissionAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string permission;

        // This is a positional argument
        public PermissionAttribute(string permission)
        {
            this.permission = permission;
        }

        public string Permission
        {
            get { return permission; }
        }
    }
}
