using Blazey.Security.Data;
using BlazeyTest.Application.Data.Security;

namespace BlazeyTest.Application.Services.DataServices.Data_User;

public class DP_User : DataPermission<User, PermissionSet, User>
{
    public override IQueryable<User> GrantedRead(User? Agent, IQueryable<User> Objects)
    {
        return Objects.Where(o => Agent != null && (o.Id == Agent.Id || Agent.Permissions.IsSysAdmin || Agent.Permissions.IsModerator));
    }

    protected override IDataPermissionResult IsChangeGranted(DataPermissionResult Permissions)
    {
        return Permissions
            .For(User => !User.Permissions.IsModerator && !User.Permissions.IsSysAdmin)
                .Deny((User, o, p) => User.Id != p.Id)
                .Deny((o, p) => p.Permissions.IsModerator || p.Permissions.IsSysAdmin)

            .For(User => !User.Permissions.IsSysAdmin)
                .Deny((o, p) => o.Permissions.IsSysAdmin || p.Permissions.IsSysAdmin)
                .Deny((o, p) => o.Permissions.IsModerator != p.Permissions.IsModerator)

            .For(User => User.Permissions.IsSysAdmin)
                .Deny((User, o, p) => User.Id != p.Id && o.Permissions.IsSysAdmin == true && p.Permissions.IsSysAdmin == false)

            .Default()
                .Deny();
    }

    protected override IDataPermissionResult IsCreationGranted(DataPermissionResultSingle Permissions)
    {
        return Permissions
            .DenyFor(User => User?.Permissions.IsSysAdmin != true);
    }

    protected override IDataPermissionResult IsDeletionGranted(DataPermissionResultSingle Permissions)
    {
        return Permissions
            .DenyFor(User => User?.Permissions.IsSysAdmin != true);
    }
}
