using Blazey.Data.DataObject;
using Blazey.Security.Identity;

namespace BlazeyTest.Application.Data.Security;

public class PermissionSet : BaseDataObject, IPermissionSet
{
    public bool IsModerator { get; set; }

    public bool IsSysAdmin { get; set; }
}
