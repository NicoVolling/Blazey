using Azure;
using Blazey.Security.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blazey.Security.Controllers;

[Route("/account/[controller]")]
[Controller]
public class AccountAPIController<T_User> : Controller where T_User : IdentityUser<Guid>, IUser, new()
{
    private readonly UserManager<T_User> userManager;

    public AccountAPIController(UserManager<T_User> userManager)
    {
        this.userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    protected virtual async Task<IdentityResult> DoRegisterAsync(T_User User, string Password)
    {
        try
        {
            IdentityResult result = await this.userManager.CreateAsync(User, Password);
            return result;
        }
        catch
        {
            throw;
        }
    }

    protected virtual void SetLastLoginMessage(string message)
    {
        Response.Cookies.Append("LastLoginMessage", message, new() { Expires = DateTime.Now.AddMinutes(1) });
    }
}