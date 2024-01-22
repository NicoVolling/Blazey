using Blazey.Data.DataObject;
using System.Security;

namespace Blazey.Security.Identity;

public interface IUser : IBaseDataObject
{
    public IPermissionSet PermissionSet { get; set; }
}