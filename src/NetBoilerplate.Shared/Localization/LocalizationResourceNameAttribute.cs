namespace NetBoilerplate.Shared.Localization;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LocalizationResourceNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
