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

public class DS_RatedList : SecuredDataService<DP_RatedList, User, PermissionSet, RatedList>
{
    DH_RatedListEntry DH_RatedListEntry;

    public DS_RatedList(IJSRuntime jSRuntime, AuthorizationService<User> authorizationService, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : base(new DH_RatedList(applicationDbContext, userManager), jSRuntime, authorizationService)
    {
        DH_RatedListEntry = new(applicationDbContext, userManager);
    }

    public void SetRating(RatedList RatedList, RatedListEntry RatedListEntry, int? Rating)
    {
        if (authoritationService.User == null) { return; }

        if (!RatedList.Entries.Contains(RatedListEntry))
        {
            RatedList.Entries.Add(RatedListEntry);
        }

        DataHandler.Change(RatedList.Id, RatedList);
        DataHandler.SaveChanges();

        ChangeRating(RatedListEntry, Rating);

        DH_RatedListEntry.Change(RatedListEntry.Id, RatedListEntry);
        DH_RatedListEntry.SaveChanges();
    }

    private void ChangeRating(RatedListEntry RatedListEntry, int? Rating)
    {
        if (authoritationService.User == null) { return; }

        if (RatedListEntry.Ratings.FirstOrDefault(o => o.UserId == authoritationService.User.Id) is RatedListEntryRating rating)
        {
            if (Rating != null)
            {
                rating.Rating = Rating.Value;
            }
            else
            {
                RatedListEntry.Ratings.Remove(rating);
            }
        }
        else if (Rating != null)
        {
            RatedListEntry.Ratings.Add(new() { Rating = Rating.Value, UserId = authoritationService.User.Id });
        }
    }
}