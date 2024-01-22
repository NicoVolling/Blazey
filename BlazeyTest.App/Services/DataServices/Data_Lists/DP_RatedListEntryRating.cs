using Blazey.Data.Services;
using Blazey.Security.Data;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Lists;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazeyTest.Application.Services.DataServices.Data_Lists;

public class DP_RatedListEntryRating : DataPermission<User, PermissionSet, RatedListEntryRating>
{
    public override IQueryable<RatedListEntryRating> GrantedRead(User? Agent, IQueryable<RatedListEntryRating> Objects)
    {
        return Objects.Where(o => true);
    }

    protected override IDataPermissionResult IsChangeGranted(DataPermissionResult Permissions)
    {
        return Permissions
            .For(User => !User.Permissions.IsModerator && !User.Permissions.IsSysAdmin)
                .Deny();
    }

    protected override IDataPermissionResult IsCreationGranted(DataPermissionResultSingle Permissions)
    {
        return Permissions;
    }

    protected override IDataPermissionResult IsDeletionGranted(DataPermissionResultSingle Permissions)
    {
        return Permissions
            .For(User => !User.Permissions.IsModerator && !User.Permissions.IsSysAdmin)
                .Deny();
    }
}