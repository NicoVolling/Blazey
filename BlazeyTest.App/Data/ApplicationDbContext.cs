using Blazey.Data.EntityFramework;
using Blazey.Security.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using BlazeyTest.Application.Data.Security;
using BlazeyTest.Application.Data.Lists;

namespace BlazeyTest.Application.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppDbContext
{
    DbContextOptions<ApplicationDbContext> options;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        this.Database.Migrate();
        this.options = options;
    }

    public DbSet<RatedListEntry> RatedListEntries { get; set; }

    public DbSet<RatedListEntryRating> RatedListEntryRatings { get; set; }

    public DbSet<RatedList> RatedLists { get; set; }

    public IAppDbContext CreateReadOnlyInstance()
    {
        return new ApplicationDbContext(options);
    }

    public void Discard()
    {
        var changedEntries = ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();

        foreach (var entry in changedEntries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
    }
}
