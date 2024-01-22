namespace Blazey.Security.Authorization;

public class BaseRules
{
    public static Rule Authenticated = new((c) => c.Principal.Identity?.IsAuthenticated == true && c.User != null);

    public static Rule IsNotAdmin = !IsSysAdmin!;

    public static Rule IsSysAdmin = Authenticated + ((c) => c.User?.PermissionSet.IsSysAdmin == true);

    public static Rule MatchAll = new((c) => true);

    public static Rule NotAuthenticated = !Authenticated;
}