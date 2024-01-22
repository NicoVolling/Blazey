using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public abstract class BaseDataPermissionResult : IDataPermissionResult
    {
        private bool isDenied;

        public bool IsGranted { get => !IsDenied; }

        protected bool IsDenied { get => isDenied; set { isDenied = value; IsReady = isDenied || IsReady; } }

        protected bool IsReady { get; set; }

        protected User? User { get; set; }

        protected Func<User, bool> UserFilter { get; set; } = o => true;
    }
}