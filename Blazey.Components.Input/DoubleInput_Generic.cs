using Microsoft.AspNetCore.Components;

namespace Blazey.Components.Input;

public partial class DoubleInput_Generic<T> : MultiCharacterInputControl<T?>
{
    [Parameter]
    public int DecimalPlaces { get; set; } = 2;

    protected override T? ConvertFromString(string? RawValue)
    {
        if (!string.IsNullOrEmpty(RawValue))
        {
            if (double.TryParse(RawValue, out double val))
            {
                return ForceConvert(val);
            }
            else
            {
                IsValueValid = false;
            }
        }
        return default;
    }

    protected override string? ConvertToString(T? Value)
    {
        return Value != null ? ForceConvert<double?>(Value)?.ToString($"N{DecimalPlaces}") : string.Empty;
    }

    protected override T? GetDefaultValue()
    {
        return ForceConvert(0);
    }

    protected override string GetInvalidMessage()
    {
        return "Format beachten: #,# (z.B.: 14,05).";
    }

    protected override bool IsValueInvalid(T? Value)
    {
        return false;
    }
}

public partial class DoubleInput : DoubleInput_Generic<double>
{ }