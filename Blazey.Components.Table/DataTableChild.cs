using Blazey.Data.DataObject;
using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Table;

public partial class DataTableChild<T> : BaseComponent where T : class, IBaseDataObject, new()
{
    [Parameter]
    public Action? __RefreshSearchQuery { get; set; }

    [Parameter]
    public bool __Render { get; set; }

    [Parameter]
    public string? __Value { get; set; }

    [CascadingParameter(Name = "CascadingDataTable")]
    public DataTable<T>? DataTable { get; set; }

    [Parameter, EditorRequired]
    public string Title { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (this is DataColumn<T> col)
        {
            DataTable?.AddColumn(col);
        }
        else if (this is IDataFilter<T> filter)
        {
            DataTable?.AddFilter(filter);
        }
    }

    protected void RefreshQuery()
    {
        __RefreshSearchQuery?.Invoke();
    }
}
