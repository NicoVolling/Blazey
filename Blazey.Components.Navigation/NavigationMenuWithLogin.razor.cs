using Blazey.Security.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Blazey.Components.Navigation;

public partial class NavigationMenuWithLogin<T_User, T_LoginPage, T_404Page> : BaseComponent where T_User : IdentityUser<Guid>, IUser
{
}