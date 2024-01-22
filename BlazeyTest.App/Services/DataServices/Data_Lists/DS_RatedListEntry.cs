using Blazey.Data.Services;
using Blazey.Security.Data.Services;
using Blazey.Security.Services;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Lists;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace BlazeyTest.Application.Services.DataServices.Data_Lists;

public class DS_RatedListEntry : SecuredDataService<DP_RatedListEntry, User, PermissionSet, RatedListEntry>
{
    public DS_RatedListEntry(IJSRuntime jSRuntime, AuthorizationService<User> authorizationService, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : base(new DH_RatedListEntry(applicationDbContext, userManager), jSRuntime, authorizationService)
    {
    }
}