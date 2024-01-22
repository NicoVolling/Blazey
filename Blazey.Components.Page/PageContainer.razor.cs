using Blazey.Security.Identity;
using Microsoft.AspNetCore.Identity;

namespace Blazey.Components.Page;

public partial class PageContainer<T_User, T_LoginPage, T_404Page> : BaseComponent where T_User : IdentityUser<Guid>, IUser
{
}