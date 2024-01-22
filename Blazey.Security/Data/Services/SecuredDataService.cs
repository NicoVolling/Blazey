using Blazey.Communications;
using Blazey.Data;
using Blazey.Data.DataObject;
using Blazey.Data.Services;
using Blazey.Security.Identity;
using Blazey.Security.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace Blazey.Security.Data.Services;

public class SecuredDataService<DataPermission, User, PermissionSet, T> : BaseDataService<T>, ISecuredDataService<T> where T : class, IBaseDataObject where User : IdentityUser<Guid>, IUser where PermissionSet : class, IPermissionSet where DataPermission : DataPermission<User, PermissionSet, T>, new()
{
    public SecuredDataService(BaseDataHandler<T> dataHandler, IJSRuntime jSRuntime, AuthorizationService<User> authorizationService) : base(dataHandler, jSRuntime)
    {
        this.authoritationService = authorizationService;
    }

    protected AuthorizationService<User> authoritationService { get; private set; }

    public bool IsChangeGranted(OperationStateProvider OperationStateProvider, T NewObject)
    {
        if (TryGetById(NewObject.Id, out T? ExistingObject, true))
        {
            if (Authorize_Write(ExistingObject, NewObject)) { return true; }
            else { OperationStateProvider.AddError(TaskResultMessageCollection.You_are_not_authorized_to_perform_this_operation()); }
        }
        return false;
    }

    public bool IsCreationGranted(OperationStateProvider OperationStateProvider, T NewObject)
    {
        if (Authorize_Write(null, NewObject)) { return true; }
        else { OperationStateProvider.AddError(TaskResultMessageCollection.You_are_not_authorized_to_perform_this_operation()); }
        return false;
    }

    public bool IsDeletionGranted(OperationStateProvider OperationStateProvider, T Object)
    {
        if (Authorize_Delete(Object)) { return true; }
        else { OperationStateProvider.AddError(TaskResultMessageCollection.You_are_not_authorized_to_perform_this_operation()); }
        return false;
    }

    protected override bool Authorize_Delete(T Object)
    {
        return new DataPermission().IsGranted(authoritationService.User, Object, null);
    }

    protected override IQueryable<T> Authorize_Read(IQueryable<T> List)
    {
        return new DataPermission().GrantedRead(authoritationService.User, List);
    }

    protected override bool Authorize_Write(T? Object, T NewObject)
    {
        return new DataPermission().IsGranted(authoritationService.User, Object, NewObject);
    }
}