namespace Blazey.Components.Input;

public class TimeInput_Generic<T> : MultiCharacterInputControl<T?>
{
    protected Dictionary<string, Func<DateTime>> FromTextDict = new()
    {
        { "Jetzt", () => DateTime.Now },
        { "+1 Stunde", () => DateTime.Now.AddHours(1) },
        { "-1 Stunde", () => DateTime.Now.AddHours(-1) },
        { "Volle Stunde", () => DateTime.Now.AddMinutes(-DateTime.Now.Minute) },
        { "+1 Volle Stunde", () => DateTime.Now.AddMinutes(-DateTime.Now.Minute).AddHours(1) },
        { "-1 Volle Stunde", () => DateTime.Now.AddMinutes(-DateTime.Now.Minute).AddHours(-1) }
    };

    protected override T? ConvertFromString(string? RawValue)
    {
        if (!string.IsNullOrEmpty(RawValue))
        {
            if (GetDateTimeByWord(RawValue, out DateTime? result))
            {
                return ForceConvert(result);
            }

            if (TimeOnly.TryParse(RawValue, out TimeOnly dt2))
            {
                return ForceConvert(new DateTime(2000, 01, 01, dt2.Hour, dt2.Minute, 0));
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
        return Value != null ? ForceConvert<DateTime?>(Value)?.ToString("HH:mm") : string.Empty;
    }

    protected override T? GetDefaultValue()
    {
        return ForceConvert(DateTime.Now);
    }

    protected override string GetInvalidMessage()
    {
        return "Format beachten: HH:mm (z.B.: 17.02.2011).";
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

public partial class TimeInput : TimeInput_Generic<DateTime?>
{ }