using Blazey.Components;
using Blazey.Components.Page;
using BlazeyTest.Application.Components.Pages.Account;
using BlazeyTest.Application.Data.Security;

namespace BlazeyTest.Application.Components.Components;

public class PageContent : PageContainer<User, Account_Login, Error_404>
{
    protected override string ApplicationName => "GameIT - Portal";
}
