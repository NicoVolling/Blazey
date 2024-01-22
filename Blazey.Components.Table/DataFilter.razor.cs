using Blazey.Components.Attributes;
using Blazey.Data.DataObject;
using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Table;

public partial class DataFilter<T, T_Value> : DataTableChild<T>, IDataFilter<T> where T : class, IBaseDataObject, new()
{
    private T_Value? max;

    private T_Value? min;

    private T_Value? value;

    [Parameter]
    public Func<IQueryable<T>, IQueryable<T>>? __Filter { get; set; }

    [Parameter]
    public bool __FilterObjectNormal { get; set; }

    [Parameter]
    public bool __FilterObjectRanged { get; set; }

    [Parameter]
    public Func<IQueryable<T>, T_Value?, IQueryable<T>>? FilterObjectNormal { get; set; }

    [Parameter]
    public Func<IQueryable<T>, T_Value?, T_Value?, IQueryable<T>>? FilterObjectRanged { get; set; }

    [QueryParameter(NameSuffixPropertyName = "Title")]
    public T_Value? Max
    { get => max; set { this.max = value; RefreshQuery(); } }

    [QueryParameter(NameSuffixPropertyName = "Title")]
    public T_Value? Min
    { get => min; set { this.min = value; RefreshQuery(); } }

    Type IDataFilter<T>.Type { get => this.GetType(); }

    [QueryParameter(NameSuffixPropertyName = "Title")]
    public T_Value? Value
    {
        get => value;
        set { this.value = value; RefreshQuery(); }
    }

    protected override bool QueryParametersEnabled => __Render;

    private (T_Value? Min, T_Value? Max)? RangedValue { get => Min == null && Max == null ? null : new(Min, Max); }

    public IQueryable<T> Filter(IQueryable<T> ObjectList)
    {
        return __Filter?.Invoke(ObjectList) ?? throw new InvalidDataException();
    }

    public void Reset()
    {
        Value = default(T_Value?);
        Min = default(T_Value?);
        Max = default(T_Value?);
        InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (!__Render)
        {
            __FilterObjectNormal = FilterObjectNormal != null;
            __FilterObjectRanged = FilterObjectRanged != null;

            __Filter = ObjectList =>
            {
                if (FilterObjectNormal != null && Value != null && !(Value is string str && string.IsNullOrEmpty(str)))
                {
                    return FilterObjectNormal(ObjectList, Value);
                }
                else if (FilterObjectRanged != null && RangedValue != null)
                {
                    return FilterObjectRanged(ObjectList, Min, Max);
                }
                return ObjectList;
            };
        }
    }
}

public static class DataFilterIQueryable_Extension
{
    public static IQueryable<T> Filter<T>(this IQueryable<T> ObjectList, IDataFilter<T> DataFilter) where T : class, IBaseDataObject
    {
        if (DataFilter.__Filter == null) { return ObjectList; }
        return DataFilter.__Filter.Invoke(ObjectList);
    }
}