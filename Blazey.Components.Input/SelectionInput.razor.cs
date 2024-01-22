using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazey.Components.Input;

public partial class SelectionInput<T> : BaseControl<T>
{
    private Guid ID;

    [Parameter, EditorRequired]
    public IEnumerable<KeyValuePair<T, string>> DataList { get; set; } = default!;

    [Parameter]
    public string? DefaultValueText { get; set; }

    [Parameter]
    public bool HideNoSelectionOption { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public bool ResetValueAfterSetter { get; set; }

    private string? RawValue { get; set; }

    protected override void Clear()
    {
        OnValueChanged(new ChangeEventArgs() { Value = default(T?) });
        InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (DefaultValueText == null)
        {
            DefaultValueText = $"(nichts ausgewählt)";
        }
        this.ID = Guid.NewGuid();
    }

    protected override void ValueUpdated()
    {
        base.ValueUpdated();
        RawValue = DataList.SingleOrDefault(o => Equals(o.Key, value)).Value;
        //JSRuntime.InvokeVoidAsync("setSelectedValue", ID.ToString("N"), DataList.SingleOrDefault(o => Equals(Value, o.Key)).Value);
    }

    private void OnValueChanged(ChangeEventArgs args)
    {
        RawValue = args.Value?.ToString();
        this.value = DataList.SingleOrDefault(o => Equals(o.Value, args.Value.ToString())).Key;

        InvokeAsync(async () => await ValueChanged.InvokeAsync(Value ?? default));

        if (ResetValueAfterSetter)
        {
            RawValue = DefaultValueText;
        }
    }
}