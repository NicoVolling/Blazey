using Blazey.Data.DataObject;

namespace Blazey.Security.Identity;

public interface IPermissionSet : IBaseDataObject
{
    public bool IsSysAdmin { get; set; }
}