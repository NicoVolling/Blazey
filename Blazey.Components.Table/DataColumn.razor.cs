using Blazey.Data.DataObject;
using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Table;

public partial class DataColumn<T> : DataTableChild<T> where T : class, IBaseDataObject, new()
{

    [Parameter]
    public RenderFragment? __ChildContent { get; set; }

    [Parameter]
    public bool __RenderSorter { get; set; }

    [Parameter]
    public RenderFragment<KeyValuePair<T, string>>? ChildContent { get; set; }

    [Parameter, EditorRequired]
    public Func<T, string> GetPropertyString { get; set; } = default!;

    [Parameter]
    public Func<IQueryable<T>, IQueryable<T>>? OrderAscending { get; set; }

    [Parameter]
    public Func<IQueryable<T>, IQueryable<T>>? OrderDescending { get; set; }

    protected override bool QueryParametersEnabled => __RenderSorter;

    public IQueryable<T> OrderQuery(IQueryable<T> Query, bool SortAscending, bool SortDescending)
    {
        if (SortAscending)
        {
            return OrderAscending?.Invoke(Query) ?? Query;
        }
        if (SortDescending)
        {
            return OrderDescending?.Invoke(Query) ?? Query;
        }
        return Query;
    }
}

public static class DataColumnIQueryable_Extension
{
    public static IQueryable<T> Order<T>(this IQueryable<T> ObjectList, DataColumn<T> DataColumn, bool SortAscending, bool SortDescending) where T : class, IBaseDataObject, new()
    {
        return DataColumn.OrderQuery(ObjectList, SortAscending, SortDescending);
    }
}
