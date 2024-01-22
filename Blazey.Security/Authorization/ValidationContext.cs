using Blazey.Security.Identity;
using System.Security.Claims;

namespace Blazey.Security.Authorization;

public class ValidationContext
{
    public ValidationContext(ClaimsPrincipal Principal, IUser? User, IEnumerable<Claim> Claims)
    {
        this.Principal = Principal;
        this.User = User;
        this.Claims = Claims;
    }

    public IEnumerable<Claim> Claims { get; set; }

    public ClaimsPrincipal Principal { get; set; }

    public IUser? User { get; set; }
}