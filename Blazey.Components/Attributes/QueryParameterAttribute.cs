namespace Blazey.Components.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute : Attribute
{
    public string? DefaultValue { get; set; }

    public string? Name { get; set; }

    public string? NameSuffixPropertyName { get; set; }
}