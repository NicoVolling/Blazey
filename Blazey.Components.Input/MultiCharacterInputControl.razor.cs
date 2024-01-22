using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Input;

public partial class MultiCharacterInputControl<T_Value> : BaseControl<T_Value>
{
    [CascadingParameter(Name = "CascadingValid")]
    public bool? CascadingValid { get; set; }

    [Parameter]
    public IEnumerable<string>? DataList { get; set; }

    public bool IsValueValid { get; set; } = true;

    [Parameter]
    public string? name { get; set; }

    [Parameter]
    public string? onkeydown { get; set; }

    [Parameter]
    public bool PreventAutocompletion { get; set; }

    [Parameter]
    public bool TextCenter { get; set; }

    [Parameter]
    public string Type { get; set; } = "text";

    protected bool Focus { get; private set; } = false;

    protected string? RawValue { get; set; }

    private Guid DataListID { get; set; }

    protected override void Clear()
    {
        InvokeAsync(async () => await UpdateRawValueAsync(null));
        InvokeAsync(StateHasChanged);
    }

    protected virtual T_Value? ConvertFromString(string? RawValue)
    {
        return default;
    }

    protected virtual string? ConvertToString(T_Value? Value)
    {
        return Value?.ToString();
    }

    protected T_Value? ForceConvert(dynamic? Value)
    {
        return ForceConvert<T_Value>(Value);
    }

    protected T? ForceConvert<T>(dynamic? Value)
    {
        return (T?)Value;
    }

    protected virtual T_Value? GetDefaultValue()
    {
        return default;
    }

    protected virtual bool IsValueInvalid(T_Value? Value)
    {
        return false;
    }

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        DataListID = Guid.NewGuid();

        RawValue = ConvertToString(Value);
        IsChecked = value != null;

        await ValueChanged.InvokeAsync(Value);
    }

    protected async Task UpdateRawValueAsync(string? Raw)
    {
        RawValue = Raw;

        IsValueValid = true;

        this.value = ConvertFromString(RawValue);

        await ValueChanged.InvokeAsync(this.Value);
    }

    protected override void ValueUpdated()
    {
        base.ValueUpdated();
        if (!Focus)
        {
            InvokeAsync(OnFocusOut);
        }
    }

    private async Task OnFocusIn()
    {
        Focus = true;
    }

    private async Task OnFocusOut()
    {
        Focus = false;

        if (IsValueInvalid(value))
        {
            Value = GetDefaultValue();
            await ValueChanged.InvokeAsync(value);
        }

        if (IsValueValid)
        {
            RawValue = ConvertToString(value);
        }
    }

    private async Task OnInputChange(ChangeEventArgs args)
    {
        await UpdateRawValueAsync((string?)args.Value);
    }
}