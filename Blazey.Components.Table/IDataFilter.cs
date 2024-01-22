using Blazey.Data.DataObject;

namespace Blazey.Components.Table;

public interface IDataFilter<T> where T : class, IBaseDataObject
{
    public Func<IQueryable<T>, IQueryable<T>>? __Filter { get; set; }

    public bool __FilterObjectNormal { get; set; }

    public bool __FilterObjectRanged { get; set; }

    public Action? __RefreshSearchQuery { get; set; }

    public bool __Render { get; set; }

    public string Title { get; set; }

    public Type Type { get; }

    public void Reset();
}
