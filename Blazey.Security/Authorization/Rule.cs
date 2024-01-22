using ValidationFunc = System.Func<Blazey.Security.Authorization.ValidationContext, bool?>;

namespace Blazey.Security.Authorization;

public class Rule
{
    public Rule(ValidationFunc ValidationFunc)
    {
        this.ValidateFunc = o =>
        {
            bool? result = ValidationFunc(o);
            return result ?? false;
        };
    }

    public Rule(params ValidationFunc[] ValidationFuncs)
    {
        this.ValidateFunc = o => ValidationFuncs.All(p => p.Invoke(o) ?? false);
    }

    private Func<ValidationContext, bool> ValidateFunc { get; init; }

    public static explicit operator ValidationFunc(Rule Rule)
    {
        return o => Rule.Validate(o);
    }

    public static Rule operator !(Rule Rule)
    {
        return new(o => !Rule.Validate(o));
    }

    public static Rule operator +(Rule Rule, ValidationFunc ValidationFunc)
    {
        return new((ValidationFunc)Rule, ValidationFunc);
    }

    public static Rule operator +(Rule Rule, Rule Rule2)
    {
        return new((ValidationFunc)Rule, (ValidationFunc)Rule2);
    }

    public bool Validate(ValidationContext context)
    {
        return ValidateFunc.Invoke(context);
    }
}