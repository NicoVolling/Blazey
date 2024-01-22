using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public abstract class BaseDataPermissionResultShared<TT> : BaseDataPermissionResult where TT : BaseDataPermissionResultShared<TT>
    {
        public TT AllowImmediateFor(Func<User?, bool> UserState)
        {
            if (IsReady) { return Next(); }

            return AllowImmediate(UserState.Invoke(User));
        }

        public TT Deny()
        {
            if (IsReady) { return Next(); }
            return Deny(true);
        }

        public TT Reset()
        {
            if (User != null && UserFilter.Invoke(User))
            {
                this.IsDenied = false;
                this.IsReady = false;
            }
            return Next();
        }

        protected TT AllowImmediate(bool Allow)
        {
            if (Allow)
            {
                this.IsDenied = false;
                this.IsReady = true;
            }
            return (this as TT)!;
        }

        protected TT AllowOnly(bool Allowed)
        {
            this.IsDenied = !Allowed;
            return (this as TT)!;
        }

        protected TT Deny(bool Denied)
        {
            this.IsDenied = Denied;
            return (this as TT)!;
        }

        protected TT Next() => (this as TT)!;
    }
}