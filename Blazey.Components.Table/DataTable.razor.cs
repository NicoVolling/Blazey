using Blazey.Communications;
using Blazey.Components.Attributes;
using Blazey.Data.DataObject;
using Blazey.Data.Services;
using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Table;

public partial class DataTable<T> : BaseComponent, IDataTable where T : class, IBaseDataObject, new()
{
    private int CurrentFilterResultCount;

    private List<DataColumn<T>>? DataColumns;

    private List<IDataFilter<T>>? DataFilters;

    private List<IDataFilter<T>>? DataFilters_Rendered;

    private IQueryable<T>? FilteredCollection;

    private bool FirstRender = true;

    private int MaxPage;

    private IEnumerable<T>? PagedCollection;

    private string SearchQuery = string.Empty;

    private string? sortColumn;
    private bool settings_ShowColumnHeader;

    [Parameter, EditorRequired]
    public RenderFragment? ChildContent { get; set; }

    public int chunkSize
    { get => ChunkSize; set { ChunkSize = value; RefreshQuery(); NavigateTo(SearchQuery); } }

    [QueryParameter(DefaultValue = "25")]
    public int ChunkSize { get; set; }

    [Parameter]
    public BaseDataService<T> DataService { get; set; } = default!;

    [Parameter]
    public Func<IQueryable<T>, IQueryable<T>> DefaultFiltering { get; set; } = o => o;

    [QueryParameter(DefaultValue = "1")]
    public int Order
    {
        get => OrderAscending ? 1 : OrderDescending ? 2 : 0;
        set { OrderAscending = value == 1; OrderDescending = value == 2; RefreshQuery(); }
    }

    public bool OrderAscending { get; set; }

    public bool OrderDescending { get; set; }

    [QueryParameter(DefaultValue = "1")]
    public int Page { get; set; }

    [QueryParameter]
    public bool Settings_HideColumnNames { get; set; }

    [QueryParameter]
    public bool Settings_ShowColumnHeader
    {
        get => settings_ShowColumnHeader;
        set => settings_ShowColumnHeader = value;
    }

    public bool Settings_ShowColumnNames { get => !Settings_HideColumnNames; set => Settings_HideColumnNames = !value; }

    [QueryParameter]
    public string? SortColumn
    { get => sortColumn; set { sortColumn = value; RefreshQuery(); } }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public bool DisableAddDelete { get; set; }

    [Parameter]
    public bool DisableDetails { get; set; }

    protected TaskResultMessageCollection TRMC { get; } = new();

    protected IQueryable<T> OnGet()
    {
        return DefaultFiltering.Invoke(DataService.Get());
    }

    public void AddColumn(DataColumn<T> Column)
    {
        if (!Column.__Render)
        {
            if (DataColumns?.Contains(Column) != true)
            {
                DataColumns?.Add(Column);
            }
            InvokeAsync(StateHasChanged);
        }
    }

    public void AddFilter(IDataFilter<T> Filter)
    {
        if (Filter.__Render)
        {
            if (DataFilters_Rendered?.Contains(Filter) != true)
            {
                DataFilters_Rendered?.Add(Filter);
            }
        }
        else
        {
            if (DataFilters?.Contains(Filter) != true)
            {
                DataFilters?.Add(Filter);
            }
        }

        InvokeAsync(StateHasChanged);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            OnLocationChanged();
            FirstRender = false;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        DataColumns = DataColumns ?? new();
        DataFilters = DataFilters ?? new();
        DataFilters_Rendered = DataFilters_Rendered ?? new();
    }

    protected override void OnLocationChanged()
    {
        if (DataFilters != null && DataColumns != null)
        {
            FilteredCollection = OnGet();
            foreach (IDataFilter<T> DataFilter in DataFilters)
            {
                if (DataFilter is BaseComponent BC) { BC.ParseQuery(); }
                FilteredCollection = FilteredCollection.Filter(DataFilter);
            }
            if (DataColumns.FirstOrDefault(o => o.Title == SortColumn) is DataColumn<T> DataColumn)
            {
                FilteredCollection = FilteredCollection.Order(DataColumn, OrderAscending, OrderDescending);
            }
            MaxPage = (int)Math.Ceiling((double)FilteredCollection.Count() / (double)chunkSize);

            if (Page > MaxPage && MaxPage > 0)
            {
                Page = MaxPage;
                RefreshQuery();
                NavigateTo(SearchQuery);
                return;
            }

            RefreshPagedCollection();

            InvokeAsync(StateHasChanged);
        }
    }

    private void RefreshPagedCollection()
    {
        PagedCollection = FilteredCollection?.Skip((Page - 1) * chunkSize).Take(chunkSize).AsEnumerable();
    }

    public Guid GetNext(Guid Current)
    {
        if (PagedCollection == null) { return Current; }
        int index = PagedCollection.Select((a, i) => (a.Id == Current) ? i : -1).Max();
        if (index == -1) { return Current; }
        if (index == PagedCollection.Count() - 1 && Page < MaxPage) { Page = Page + 1; RefreshPagedCollection(); return PagedCollection.First().Id; }
        if (index == PagedCollection.Count() - 1) { return Current; }
        return PagedCollection.ElementAt(index + 1).Id;
    }
    public Guid GetPrevious(Guid Current)
    {
        if (PagedCollection == null) { return Current; }
        int index = PagedCollection.Select((a, i) => (a.Id == Current) ? i : -1).Max();
        if (index == -1) { return Current; }
        if (index == 0 && Page > 1) { Page = Page - 1; RefreshPagedCollection(); return PagedCollection.Last().Id; }
        if (index == 0) { return Current; }
        return PagedCollection.ElementAt(index - 1).Id;
    }

    private void CalculateResultCount()
    {
        if (DataFilters != null)
        {
            IQueryable<T> FilteredCollection = OnGet();
            foreach (IDataFilter<T> DataFilter in DataFilters)
            {
                if (DataFilter is BaseComponent BC) { BC.ParseQuery(SearchQuery); }
                FilteredCollection = FilteredCollection.Filter(DataFilter);
            }
            CurrentFilterResultCount = FilteredCollection.Count();
            InvokeAsync(StateHasChanged);
        }
    }

    private string GenerateEntryTexts()
    {
        int FullCount = DataService.Count();
        string result = TRMC.X0_results_found_SG_PL(CurrentFilterResultCount);

        if (CurrentFilterResultCount > 0 && FullCount - CurrentFilterResultCount > 0)
        {
            result += $" {TRMC.There_are_X0_entries_that_do_not_match_the_conditions_SG_PL(FullCount - CurrentFilterResultCount)}";
        }
        else if (CurrentFilterResultCount < FullCount && FullCount > 1)
        {
            result += $" {TRMC.None_of_the_X0_entries_match_the_conditions(FullCount)}";
        }
        else if (FullCount == CurrentFilterResultCount)
        {
            if (CurrentFilterResultCount > 1)
            {
                result += $" {TRMC.All_entries_match_the_conditions()}.";
            }
            else if (CurrentFilterResultCount == 1)
            {
                result += $" {TRMC.There_is_only_one_entry()}";
            }
            else
            {
                result += $" {TRMC.There_is_no_entry()}";
            }
        }
        return result;
    }

    private IEnumerable<KeyValuePair<string, string>> GetColumnTitles()
    {
        if (DataColumns == null) { throw new NullReferenceException(); }
        var list = DataColumns.Select(o => new KeyValuePair<string, string>(o.Title, o.Title));
        return list;
    }

    private Dictionary<string, object?> GetParametersDataColumn(DataColumn<T> DataColumn, T Value)
    {
        return new Dictionary<string, object?>()
        {
            { nameof(DataColumn.Title), DataColumn.Title },
            { nameof(DataColumn.GetPropertyString), DataColumn.GetPropertyString },
            { nameof(DataColumn.__Render), true },
            { nameof(DataColumn.__ChildContent), DataColumn.ChildContent?.Invoke(new(Value, DataColumn.GetPropertyString(Value))) },
            { nameof(DataColumn.__Value), DataColumn.GetPropertyString(Value) }
        };
    }

    private Dictionary<string, object?> GetParametersDataFilter(IDataFilter<T> DataFilter)
    {
        return new Dictionary<string, object?>()
        {
            { nameof(DataFilter.__Filter), DataFilter.__Filter },
            { nameof(DataFilter.Title), DataFilter.Title },
            { nameof(DataFilter.__FilterObjectNormal), DataFilter.__FilterObjectNormal },
            { nameof(DataFilter.__FilterObjectRanged), DataFilter.__FilterObjectRanged },
            { nameof(DataFilter.__RefreshSearchQuery), () => { OnLocationChanged(); } },
            { nameof(DataTableChild<T>.__Render), true }
        };
    }

    private void RefreshQuery()
    {
        SearchQuery = GetQuery((nameof(Page), 1));
        CalculateResultCount();
        InvokeAsync(StateHasChanged);
    }

    private void ResetFilter()
    {
        if (DataFilters_Rendered != null)
        {
            foreach (IDataFilter<T> DataFilter in DataFilters_Rendered)
            {
                DataFilter.Reset();
            }
            NavigateTo(GetQuery(), true);
        }
    }
}
