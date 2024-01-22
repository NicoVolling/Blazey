using Blazey.Components;
using Blazey.Data.EntityFramework;
using Blazey.Security.Authorization;
using Blazey.Security.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blazey.Security.Components;

public partial class Authorize<T_User, T_LoginPage, T_404Page> : BaseComponent where T_User : IdentityUser<Guid>, IUser
{
    public Authorize()
    {
        ValidateWhen = () => true;
    }

    [Parameter]
    public RenderFragment<T_User?>? ChildContent { get; set; }

    public bool Display { get; set; }

    [Parameter]
    public bool PageRequirement { get; set; }

    [Parameter]
    public bool Show404NotAuthorized { get; set; }

    [Parameter]
    public bool ShowLoginIfNotAuthorized { get; set; }

    [Parameter]
    public Func<bool> ValidateWhen { get; set; }

    [Parameter]
    public Rule ValidationRule { get; set; } = BaseRules.MatchAll;

    [Inject]
    protected IAppDbContext AppDbContext { get; set; } = default!;

    [CascadingParameter]
    protected Task<AuthenticationState> authenticationStateTask { get; set; } = default!;

    [Inject]
    protected UserManager<T_User> userManager { get; set; } = default!;

    private T_User? User { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (ValidateWhen())
        {
            this.Display = await ValidateAsync();
        }
        else
        {
            this.Display = true;
            this.User = null;
        }
    }

    private async Task<bool> ValidateAsync()
    {
        return await ValidateAsync(ValidationRule);
    }

    private async Task<bool> ValidateAsync(Rule Rule)
    {
        if ((await authenticationStateTask).User is ClaimsPrincipal CP)
        {
            Guid id = userManager.GetUserAsync(CP).Result?.Id ?? Guid.Empty;

            if (AppDbContext is IdentityDbContext<T_User, IdentityRole<Guid>, Guid> idbc)
            {
                this.User = idbc.Users.Include("Permissions").FirstOrDefault(o => o.Id == id);
            }

            bool result = Rule.Validate(new(CP, this.User, CP.Claims));
            return result;
        }
        return false;
    }
}