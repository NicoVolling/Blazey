using Blazey.Data.Services;
using Blazey.Security.Authorization;
using Blazey.Security.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Blazey.Security.Services;

public class AuthorizationService<T_User> where T_User : IdentityUser<Guid>, IUser
{
    private readonly AuthenticationStateProvider authenticationStateProvider;

    private readonly UserManager<T_User> userManager;

    private T_User? user;

    public AuthorizationService(AuthenticationStateProvider authenticationStateProvider, UserManager<T_User> userManager, BaseDataHandler<T_User> DH_User)
    {
        this.authenticationStateProvider = authenticationStateProvider;
        this.userManager = userManager;
        this.DH_User = DH_User;
        authenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
        this.User = GetCurrentUser();
    }

    public ClaimsPrincipal ClaimsPrincipal { get; private set; } = new();

    public BaseDataHandler<T_User> DH_User { get; }

    public bool IsAdmin { get => User?.PermissionSet.IsSysAdmin == true; }

    public bool IsAuthenticated { get => User != null; }

    public T_User? User { get { if (UserFetched == null || UserFetched.Value.AddMinutes(15) < DateTime.Now) { user = GetCurrentUser(); } return user; } private set => user = value; }

    public DateTime? UserFetched { get; private set; }

    public bool Authorize(Rule Rule)
    {
        return Rule.Validate(new(ClaimsPrincipal, User, new List<Claim>()));
    }

    public T_User? GetCurrentUser(Task<AuthenticationState>? Task = null)
    {
        UserFetched = DateTime.Now;
        AuthenticationState authstate = Task?.Result ?? authenticationStateProvider.GetAuthenticationStateAsync().Result;
        this.ClaimsPrincipal = authstate.User;
        string? id = userManager.GetUserId(ClaimsPrincipal);
        if (DH_User.TryGetById(id ?? string.Empty, out T_User? User, true))
        {
            return User;
        }
        return null;
    }

    private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
        this.User = GetCurrentUser(task);
    }
}