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

public class DS_RatedListEntryRating : SecuredDataService<DP_RatedListEntryRating, User, PermissionSet, RatedListEntryRating>
{
    public DS_RatedListEntryRating(IJSRuntime jSRuntime, AuthorizationService<User> authorizationService, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : base(new DH_RatedListEntryRating(applicationDbContext, userManager), jSRuntime, authorizationService)
    {
    }
}