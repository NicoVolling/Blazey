namespace Blazey.Components.Input;

public partial class DateInput_Generic<T> : MultiCharacterInputControl<T?>
{
    protected Dictionary<string, Func<DateTime>> FromTextDict = new()
    {
        { "Heute", () => DateTime.Now },
        { "Morgen", () => DateTime.Now.AddDays(1) },
        { "Gestern", () => DateTime.Now.AddDays(-1) },
        { "Übermorgen", () => DateTime.Now.AddDays(2) },
        { "Vorgestern", () => DateTime.Now.AddDays(-2) },
        { "Vormonat", () => DateTime.Now.AddMonths(-1) },
        { "Folgemonat", () => DateTime.Now.AddMonths(1) },
    };

    protected override T? ConvertFromString(string? RawValue)
    {
        if (!string.IsNullOrEmpty(RawValue))
        {
            if (GetDateTimeByWord(RawValue, out DateTime? result))
            {
                return ForceConvert(result);
            }

            if (DateTime.TryParse(RawValue, out DateTime dt2))
            {
                return ForceConvert(dt2);
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
        return Value != null ? ForceConvert<DateTime?>(Value)?.ToString("dd.MM.yyyy") : string.Empty;
    }

    protected override T? GetDefaultValue()
    {
        return ForceConvert(DateTime.Now);
    }

    protected override string GetInvalidMessage()
    {
        return "Format beachten: TT.MM.JJJJ (z.B.: 17.02.2011).";
    }

    protected override bool IsValueInvalid(T? Value)
    {
        return ForceConvert<DateTime?>(Value)?.Year == 1;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (DataList == null)
        {
            DateTime now = DateTime.Now;
            this.DataList = FromTextDict.OrderBy(o => o.Value.Invoke()).Select(o => o.Key).ToList();
        }
    }

    private bool GetDateTimeByWord(string Value, out DateTime? Result)
    {
        if (FromTextDict.FirstOrDefault(o => o.Key.ToLower() == RawValue.ToLower().Trim()) is KeyValuePair<string, Func<DateTime>> kvp && kvp.Value != null)
        {
            DateTime dt = kvp.Value.Invoke();
            RawValue = ConvertToString(ForceConvert(dt));
            Result = dt;
            return true;
        }

        Result = null;
        return false;
    }
}

public partial class DateInput : DateInput_Generic<DateTime?>
{ }