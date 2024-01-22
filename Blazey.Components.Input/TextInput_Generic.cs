namespace Blazey.Components.Input;

public partial class TextInput_Generic<T> : MultiCharacterInputControl<T>
{
    protected override T? ConvertFromString(string? RawValue)
    {
        return (T?)(dynamic?)RawValue;
    }

    protected override string? ConvertToString(T? Value)
    {
        return Value as string;
    }

    protected override T? GetDefaultValue()
    {
        return ForceConvert<T>(string.Empty);
    }

    protected override bool IsValueInvalid(T? Value)
    {
        return false;
    }
}

public partial class TextInput : TextInput_Generic<string>
{ }