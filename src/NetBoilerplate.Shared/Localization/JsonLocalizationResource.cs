namespace NetBoilerplate.Shared.Localization;

internal sealed class JsonLocalizationResource
{
    public string Culture { get; set; } = string.Empty;
    public Dictionary<string, string> Texts { get; set; } = [];
}
