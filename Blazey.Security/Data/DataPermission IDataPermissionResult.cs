using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public interface IDataPermissionResult
    {
        public bool IsGranted { get; }
    }
}