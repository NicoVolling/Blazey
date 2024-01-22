using Blazey.Communications;
using Blazey.Security.Data.Services;
using Blazey.Security.Services;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace BlazeyTest.Application.Services.DataServices.Data_User;

public class DS_User : SecuredDataService<DP_User, User, PermissionSet, User>
{
    private readonly DP_User Permissions;

    public DS_User(ApplicationDbContext dbContext, UserManager<User> userManager, IJSRuntime _jsruntime, AuthorizationService<User> authorizationSerivce) : base(new DH_User(dbContext, userManager), _jsruntime, authorizationSerivce)
    {
        Permissions = new();
    }

    public bool DoesAdminExist()
    {
        return OnGet().Where(o => o.Permissions.IsSysAdmin).Count() > 0;
    }

    protected override OperationStateProvider OnChange(OperationStateProvider OperationState, Guid Id, User ExistingObject, User NewObject)
    {
        NewObject.NormalizedUserName = NewObject.UserName?.ToUpper();
        NewObject.NormalizedEmail = NewObject.Email?.ToUpper();
        return base.OnChange(OperationState, Id, ExistingObject, NewObject);
    }

    protected override OperationStateProvider OnCreate(OperationStateProvider OperationState, User NewObject)
    {
        NewObject.NormalizedUserName = NewObject.UserName?.ToUpper();
        NewObject.NormalizedEmail = NewObject.Email?.ToUpper();
        return base.OnCreate(OperationState, NewObject);
    }
}
