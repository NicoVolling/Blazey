using Blazey.Security.Identity;
using Microsoft.AspNetCore.Identity;

namespace Blazey.Security.Components;

public partial class Account_Confirm<T_User, T_404Page> where T_User : IdentityUser<Guid>, IUser
{
}