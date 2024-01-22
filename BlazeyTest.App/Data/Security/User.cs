using Blazey.Data.DataObject;
using Blazey.Security.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeyTest.Application.Data.Security;

public class User : IdentityUser<Guid>, IBaseDataObject, IInputDataObjectProvider<UserInput>, IUser
{
    public string? Firstname { get; set; }

    [NotMapped]
    public UserInput InputDataObject { get; set; } = new();

    public string? Lastname { get; set; }

    public PermissionSet Permissions { get; set; } = new();

    [NotMapped]
    public IPermissionSet PermissionSet { get => Permissions; set => Permissions = value as PermissionSet ?? new(); }

    public T Clone<T>() where T : class, IBaseDataObject
    {
        return (T)this.MemberwiseClone();
    }

    public void Reset()
    {
        InputDataObject = new();
    }
}
