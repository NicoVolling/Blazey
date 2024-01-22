using Blazey.Data.Services;
using BlazeyTest.Application.Data;
using BlazeyTest.Application.Data.Lists;
using BlazeyTest.Application.Data.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazeyTest.Application.Services.DataServices.Data_Lists;

public class DH_RatedList : BaseDataHandler<RatedList>
{
    public DH_RatedList(ApplicationDbContext DbContext, UserManager<User> userManager) : base(DbContext)
    {
    }

    public override IQueryable<RatedList> Get(bool ReadOnly = false)
    {
        return base.Get(ReadOnly).Include(o => o.Entries).ThenInclude(o => o.Ratings);
    }
}