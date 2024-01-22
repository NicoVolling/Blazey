using Blazey.Data.Services;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Lists;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazeyTest.Application.Services.DataServices.Data_Lists;

public class DH_RatedListEntry : BaseDataHandler<RatedListEntry>
{
    public DH_RatedListEntry(ApplicationDbContext DbContext, UserManager<User> userManager) : base(DbContext)
    {
    }
}