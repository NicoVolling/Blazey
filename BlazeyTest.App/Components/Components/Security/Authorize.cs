using Blazey.Components;
using Blazey.Security.Components;
using BlazeyTest.Application.Components.Pages.Account;
using BlazeyTest.Application.Data.Security;

namespace BlazeyTest.Application.Components.Components.Security;

public class Authorize : Authorize<User, Account_Login, Error_404>
{
}
