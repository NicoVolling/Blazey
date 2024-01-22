global using Rules = BlazeyTest.Application.Security.Rules;
using Blazey.Security.Authorization;
using BlazeyTest.Application.Data.Security;

namespace BlazeyTest.Application.Security;

public class Rules : BaseRules
{
    public static Rule Permission_Moderator = Authenticated + ((c) => ((PermissionSet?)c.User?.PermissionSet)?.IsSysAdmin == true || ((PermissionSet?)c.User?.PermissionSet)?.IsModerator == true);

    public static Rule Permission_ModeratorOnly = Authenticated + ((c) => ((PermissionSet?)c.User?.PermissionSet)?.IsModerator == true);
}
