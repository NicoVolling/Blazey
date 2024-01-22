using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public class DataPermissionResultUserSafeSingle : BaseDataPermissionResultShared<DataPermissionResultUserSafeSingle>
    {
        public DataPermissionResultUserSafeSingle(User User, T Object, bool IsDenied, bool IsReady, Func<User, bool> UserFilter)
        {
            this.IsReady = IsReady;
            this.IsDenied = IsDenied;
            this.NewUser = User;
            this.User = User;
            this.Object = Object;
            this.UserFilter = UserFilter;
        }

        public T Object { get; }

        protected bool Hit { get; set; }

        protected User NewUser { get; }

        public DataPermissionResultUserSafeSingle AllowOnly(Func<T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            return AllowOnly(State.Invoke(Object));
        }

        public DataPermissionResultSingle Default()
        {
            if (!Hit)
            {
                UserFilter = o => true;
                return ToUserUnsafe(Next());
            }
            else
            {
                UserFilter = o => true;
                IsReady = true;
                return ToUserUnsafe(Next());
            }
        }

        public DataPermissionResultUserSafeSingle Deny(Func<T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            return Deny(State.Invoke(Object));
        }

        public DataPermissionResultUserSafeSingle Deny(Func<User, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            return Deny(State.Invoke(NewUser, Object));
        }

        public DataPermissionResultUserSafeSingle For(Func<User, bool> UserFilter)
        {
            if (IsReady) { return Next(); }
            if (User == null) { return Next(); }

            this.UserFilter = UserFilter;
            return Next();
        }

        public DataPermissionResultSingle ForAll()
        {
            UserFilter = o => true;
            return ToUserUnsafe(Next());
        }

        public DataPermissionResultUserSafeSingle Require(Func<PermissionSet, bool> UserState)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            return AllowOnly(UserState.Invoke((PermissionSet)NewUser.PermissionSet));
        }

        public DataPermissionResultUserSafeSingle RequireAllowOnly(Func<PermissionSet, bool> UserState, Func<T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            return AllowOnly(UserState.Invoke((PermissionSet)NewUser.PermissionSet) && State.Invoke(Object));
        }

        protected DataPermissionResultSingle ToUserUnsafe(DataPermissionResultUserSafeSingle DPR)
        {
            return new DataPermissionResultSingle(DPR.User!, DPR.Object, DPR.IsDenied, DPR.IsReady, DPR.UserFilter);
        }
    }
}