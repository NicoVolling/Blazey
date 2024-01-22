using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Input;

public class BaseControl<T_Value> : BaseComponent
{
    protected T_Value? value;

    private bool isChecked;

    [Parameter]
    public bool Check { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter(Name = "CascadingDisabled"), Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public bool FullWidth { get; set; }

    [Parameter]
    public bool HideTitle { get; set; }

    [Parameter]
    public string? Pre { get; set; }

    [CascadingParameter(Name = "ReadOnly"), Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public bool Small { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public virtual T_Value? Value
    {
        get => value;
        set { bool changed = !CompareTwoValues(this.value, value); this.value = value; if (changed) { ValueUpdated(); } }
    }

    [Parameter]
    public EventCallback<T_Value?> ValueChanged { get; set; }

    protected bool IsChecked
    { get => isChecked; set { isChecked = value; if (!isChecked) { Clear(); } } }

    protected virtual string GetInvalidMessage() { return "Eingabe ungültig."; }

    protected virtual void Clear()
    {
    }

    protected virtual void ValueUpdated()
    {
    }

    private bool CompareTwoValues(object? value1, object? value2)
    {
        if (value1 is null && value2 is null)
        {
            return true;
        }
        else if (value1 is null || value2 is null)
        {
            return false;
        }
        else if (ReferenceEquals(value1, value2))
        {
            return true;
        }
        else if (value1 == value2)
        {
            return true;
        }
        return false;
    }
}