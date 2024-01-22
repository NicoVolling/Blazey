namespace Blazey.Components.Input;

public partial class IntegerInput_Generic<T> : MultiCharacterInputControl<T?>
{
    protected override T? ConvertFromString(string? RawValue)
    {
        if (!string.IsNullOrEmpty(RawValue))
        {
            if (int.TryParse(RawValue, out int val))
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
        return Value != null ? ForceConvert<int?>(Value)?.ToString() : string.Empty;
    }

    protected override T? GetDefaultValue()
    {
        return ForceConvert(0);
    }

    protected override string GetInvalidMessage()
    {
        return "Format beachten: # (z.B.: 181).";
    }

    protected override bool IsValueInvalid(T? Value)
    {
        return false;
    }
}

public partial class IntegerInput : IntegerInput_Generic<int>
{ }