using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public class DataPermissionResult : BaseDataPermissionResultShared<DataPermissionResult>
    {
        public DataPermissionResult(User? User, T ExistingObject, T NewObject)
        {
            this.User = User;
            this.ExistingObject = ExistingObject;
            this.NewObject = NewObject;
        }

        public DataPermissionResult(User? User, T ExistingObject, T NewObject, bool IsDenied, bool IsReady, Func<User, bool> UserFilter)
        {
            this.IsReady = IsReady;
            this.IsDenied = IsDenied;
            this.User = User;
            this.ExistingObject = ExistingObject;
            this.NewObject = NewObject;
            this.UserFilter = UserFilter;
        }

        public T ExistingObject { get; }

        public T NewObject { get; }

        public DataPermissionResult AllowImmediateFor(Func<User?, bool> UserState, Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowImmediate(State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResult AllowImmediateFor(Func<User?, bool> UserState, Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowImmediate(StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        public DataPermissionResult AllowOnlyFor(Func<User?, bool> UserState, Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowOnly(State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResult AllowOnlyFor(Func<User?, bool> UserState, Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return AllowOnly(StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        public DataPermissionResult Deny(Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResult Deny(Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        public DataPermissionResult Deny(Func<User?, T, T, bool> State)
        {
            if (IsReady) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(State.Invoke(User, ExistingObject, NewObject));
        }

        public DataPermissionResult DenyFor(Func<User?, bool> UserState, Func<T, T, bool> State)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(State.Invoke(ExistingObject, NewObject));
        }

        public DataPermissionResult DenyFor(Func<User?, bool> UserState, Func<T, bool> StateExisting, Func<T, bool> StateNew)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            if (ExistingObject == null || NewObject == null) { return Deny(); }
            return Deny(StateExisting.Invoke(ExistingObject) && StateNew.Invoke(NewObject));
        }

        public DataPermissionResult DenyFor(Func<User?, bool> UserState)
        {
            if (IsReady) { return Next(); }
            if (!UserState.Invoke(User)) { return Next(); }

            return Deny();
        }

        public DataPermissionResultUserSafe For(Func<User, bool> UserFilter)
        {
            if (IsReady) { return ToUserSafe(Next()); }
            if (User == null) { return ToUserSafe(Next()); }

            this.UserFilter = UserFilter;
            return ToUserSafe(Next());
        }

        protected DataPermissionResultUserSafe ToUserSafe(DataPermissionResult DPR)
        {
            return new DataPermissionResultUserSafe(DPR.User!, DPR.ExistingObject, DPR.NewObject, DPR.IsDenied, DPR.IsReady, DPR.UserFilter);
        }
    }
}