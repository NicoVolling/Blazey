using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace Blazey.Data.EntityFramework;

public interface IAppDbContext
{
    public ChangeTracker ChangeTracker { get; }

    public static void Discard(DbContext DbContext)
    {
        if (DbContext.ChangeTracker.HasChanges())
        {
            foreach (EntityEntry Entry in DbContext.ChangeTracker.Entries())
            {
                Entry.CurrentValues.SetValues(Entry.OriginalValues);
                Entry.State = EntityState.Unchanged;
            }
        }
    }

    public IAppDbContext CreateReadOnlyInstance();

    public void Discard();

    public EntityEntry<T> Entry<T>(T Entity) where T : class;

    public int SaveChanges();

    public DbSet<T> Set<T>() where T : class;
}