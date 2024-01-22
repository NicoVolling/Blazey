using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public class DataPermissionResultSingle : BaseDataPermissionResultShared<DataPermissionResultSingle>
    {
        public DataPermissionResultSingle(User? User, T Object)
        {
            this.User = User;
            this.Object = Object;
        }

        public DataPermissionResultSingle(User? User, T Object, bool IsDenied, bool IsReady, Func<User, bool> UserFilter)
        {
            this.IsReady = IsReady;
            this.IsDenied = IsDenied;
            this.User = User;
            this.Object = Object;
            this.UserFilter = UserFilter;
        }

        public T Object { get; }

        public DataPermissionResultSingle AllowImmediateFor(Func<User?, bool> UserState, Func<T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            return AllowImmediate(State.Invoke(Object));
        }

        public DataPermissionResultSingle AllowOnlyFor(Func<User?, bool> UserState, Func<T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            return AllowOnly(State.Invoke(Object));
        }

        public DataPermissionResultSingle Deny(Func<T, bool> State)
        {
            if (IsReady) { return Next(); }

            return Deny(State.Invoke(Object));
        }

        public DataPermissionResultSingle Deny(Func<User?, T, bool> State)
        {
            if (IsReady) { return Next(); }

            return Deny(State.Invoke(User, Object));
        }

        public DataPermissionResultSingle DenyFor(Func<User?, bool> UserState, Func<T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            return Deny(State.Invoke(Object));
        }

        public DataPermissionResultSingle DenyFor(Func<User?, bool> UserState)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            return Deny();
        }

        public DataPermissionResultUserSafeSingle For(Func<User, bool> UserFilter)
        {
            if (IsReady) { return ToUserSafe(Next()); }
            if (User == null) { return ToUserSafe(Next()); }

            this.UserFilter = UserFilter;
            return ToUserSafe(Next());
        }

        protected DataPermissionResultUserSafeSingle ToUserSafe(DataPermissionResultSingle DPR)
        {
            return new DataPermissionResultUserSafeSingle(DPR.User!, DPR.Object, DPR.IsDenied, DPR.IsReady, DPR.UserFilter);
        }
    }
}