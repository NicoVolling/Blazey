using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public class DataPermissionResultUserSafe : BaseDataPermissionResultShared<DataPermissionResultUserSafe>
    {
        public DataPermissionResultUserSafe(User User, T ExistingObject, T NewObject, bool IsDenied, bool IsReady, Func<User, bool> UserFilter)
        {
            this.IsReady = IsReady;
            this.IsDenied = IsDenied;
            this.User = User;
            this.NewUser = User;
            this.ExistingObject = ExistingObject;
            this.NewObject = NewObject;
            this.UserFilter = UserFilter;
        }

        public T ExistingObject { get; }

        public T NewObject { get; }

        protected bool Hit { get; set; }

        protected User NewUser { get; }

        public DataPermissionResultUserSafe AllowOnly(Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowOnly(State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResultUserSafe AllowOnly(Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowOnly(StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        public DataPermissionResult Default()
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

        public DataPermissionResultUserSafe Deny(Func<User, T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(State.Invoke(NewUser, ExistingObject, NewObject));
        }

        public DataPermissionResultUserSafe Deny(Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        public DataPermissionResultUserSafe Deny(Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResultUserSafe For(Func<User, bool> UserFilter)
        {
            if (IsReady) { return Next(); }
            if (User == null) { return Next(); }

            this.UserFilter = UserFilter;
            return Next();
        }

        public DataPermissionResult ForAll()
        {
            UserFilter = o => true;
            return ToUserUnsafe(Next());
        }

        public DataPermissionResultUserSafe Require(Func<PermissionSet, bool> UserState)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            return AllowOnly(UserState.Invoke((PermissionSet)NewUser.PermissionSet));
        }

        public DataPermissionResultUserSafe RequireAllowOnly(Func<PermissionSet, bool> UserState, Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowOnly(UserState.Invoke((PermissionSet)NewUser.PermissionSet) && State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResultUserSafe RequireAllowOnly(Func<PermissionSet, bool> UserState, Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }
            if (!UserFilter.Invoke(NewUser)) { return Next(); } else { Hit = true; }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowOnly(UserState.Invoke((PermissionSet)NewUser.PermissionSet) && StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        protected DataPermissionResult ToUserUnsafe(DataPermissionResultUserSafe DPR)
        {
            return new DataPermissionResult(DPR.User!, DPR.ExistingObject, DPR.NewObject, DPR.IsDenied, DPR.IsReady, DPR.UserFilter);
        }
    }
}