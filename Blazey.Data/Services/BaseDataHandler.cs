using Blazey.Communications;
using Blazey.Data.DataObject;
using Blazey.Data.EntityFramework;

namespace Blazey.Data.Services;

public class BaseDataHandler<T> where T : class, IBaseDataObject
{
    public BaseDataHandler(IAppDbContext DbContext)
    {
        this.DbContext = DbContext;
        this.TaskResultMessageCollection = new();
    }

    public IAppDbContext DbContext { get; }

    public TaskResultMessageCollection TaskResultMessageCollection { get; }

    public void Change(Guid Id, T NewObject)
    {
        if (TryGetById(Id, out T? ExistingObject) && ExistingObject != null)
        {
            OnChange(ExistingObject, NewObject);
            ResetInputDataObject(ExistingObject);
        }
        else
        {
            throw new ArgumentException(TaskResultMessageCollection.This_object_does_not_exist());
        }
    }

    public virtual void Create(T NewObject)
    {
        DbContext.Set<T>().Add(NewObject);
        ResetInputDataObject(NewObject);
    }

    public virtual void Delete(Guid Id)
    {
        if (TryGetById(Id, out T? ExistingObject) && ExistingObject != null)
        {
            DbContext.Set<T>().Remove(ExistingObject!);
        }
        else
        {
            throw new ArgumentException(TaskResultMessageCollection.This_object_does_not_exist());
        }
    }

    public virtual IQueryable<T> Get(bool ReadOnly = false)
    {
        return (ReadOnly ? DbContext.CreateReadOnlyInstance() : DbContext).Set<T>();
    }

    public virtual void SaveChanges()
    {
        DbContext.SaveChanges();
    }

    public virtual bool TryGetById(IQueryable<T> Objects, Guid Id, out T? Result)
    {
        Result = Objects.SingleOrDefault(o => o.Id == Id);
        return Result is T;
    }

    public virtual bool TryGetById(IQueryable<T> Objects, string Id, out T? Result)
    {
        if (Guid.TryParse(Id, out Guid result))
        {
            return TryGetById(Objects, result, out Result);
        }
        else
        {
            Result = null;
            return false;
        }
    }

    public bool TryGetById(Guid Id, out T? Result, bool ReadOnly = false)
    {
        return TryGetById(this.Get(ReadOnly), Id, out Result);
    }

    public bool TryGetById(string Id, out T? Result, bool ReadOnly = false)
    {
        if (Guid.TryParse(Id, out Guid result))
        {
            return TryGetById(result, out Result, ReadOnly);
        }
        else
        {
            Result = null;
            return false;
        }
    }

    protected virtual void OnChange(T ExistingObject, T NewObject)
    {
        DbContext.Entry(ExistingObject!).CurrentValues.SetValues(NewObject);

        foreach (var navObj in DbContext.Entry(NewObject).Navigations)
        {
            foreach (var navExist in DbContext.Entry(ExistingObject).Navigations)
            {
                if (navObj.Metadata.Name == navExist.Metadata.Name)
                {
                    navExist.CurrentValue = navObj.CurrentValue;
                }
            }
        }
    }

    protected virtual void ResetInputDataObject(T Object)
    {
        if (Object is IBaseInputDataObjectProvier IBIDOP)
        {
            IBIDOP.Reset();
        }
    }
}