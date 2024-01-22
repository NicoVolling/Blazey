using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace Blazey.Security.Data;

public partial class DataPermission<User, PermissionSet, T> where T : class, IBaseDataObject where User : class, IUser where PermissionSet : class, IPermissionSet
{
    public virtual IQueryable<T> GrantedRead(User? Agent, IQueryable<T> Objects) => Objects;

    public bool IsGranted(User? Agent, T? ExistingObject, T? NewObject)
    {
        if (ExistingObject == null && NewObject != null)
        {
            return IsCreationGranted(new(Agent, NewObject)).IsGranted;
        }
        else if (ExistingObject != null && NewObject != null)
        {
            return IsChangeGranted(new(Agent, ExistingObject, NewObject)).IsGranted;
        }
        else if (ExistingObject != null && NewObject == null)
        {
            return IsDeletionGranted(new(Agent, ExistingObject)).IsGranted;
        }
        return false;
    }

    protected virtual IDataPermissionResult IsChangeGranted(DataPermissionResult Permissions) => Permissions;

    protected virtual IDataPermissionResult IsCreationGranted(DataPermissionResultSingle Permissions) => Permissions;

    protected virtual IDataPermissionResult IsDeletionGranted(DataPermissionResultSingle Permissions) => Permissions;
}