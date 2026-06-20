using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace NetBoilerplate.Shared.Localization;

public sealed class JsonLocalizationStore
{
    private const string LocalizationPath = ".Localization.";
    private const string DefaultCulture = "en";
    private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _resources = [];

    public JsonLocalizationStore(Assembly assembly)
    {
        foreach (var resourcePath in assembly.GetManifestResourceNames()
                     .Where(path => path.Contains(LocalizationPath, StringComparison.Ordinal) &&
                                    path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
        {
            using var stream = assembly.GetManifestResourceStream(resourcePath)
                               ?? throw new InvalidOperationException($"Embedded resource '{resourcePath}' was not found.");
            var resource = JsonSerializer.Deserialize<JsonLocalizationResource>(stream,
                               new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                           ?? throw new InvalidOperationException($"Embedded resource '{resourcePath}' is invalid.");

            var resourceName = GetResourceName(resourcePath, resource.Culture);
            if (!_resources.TryGetValue(resourceName, out var cultures))
            {
                cultures = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                _resources[resourceName] = cultures;
            }

            cultures[resource.Culture] = new Dictionary<string, string>(resource.Texts, StringComparer.Ordinal);
        }
    }

    public string? GetString(string resourceName, string name, CultureInfo culture)
    {
        foreach (var cultureName in GetCultureNames(culture, true))
        {
            if (GetTexts(resourceName, cultureName).TryGetValue(name, out var value))
            {
                return value;
            }
        }

        return null;
    }

    public IReadOnlyDictionary<string, string> GetAllStrings(
        string resourceName,
        CultureInfo culture,
        bool includeParentCultures)
    {
        var texts = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var cultureName in GetCultureNames(culture, includeParentCultures).Reverse())
        {
            foreach (var (name, value) in GetTexts(resourceName, cultureName))
            {
                texts[name] = value;
            }
        }

        return texts;
    }

    private static IEnumerable<string> GetCultureNames(CultureInfo culture, bool includeParentCultures)
    {
        var cultureNames = new List<string>();

        if (!string.IsNullOrWhiteSpace(culture.Name))
        {
            cultureNames.Add(culture.Name);
        }

        if (includeParentCultures)
        {
            var parent = culture.Parent;
            while (!string.IsNullOrWhiteSpace(parent.Name))
            {
                cultureNames.Add(parent.Name);
                parent = parent.Parent;
            }
        }

        if (!cultureNames.Contains(DefaultCulture, StringComparer.OrdinalIgnoreCase))
        {
            cultureNames.Add(DefaultCulture);
        }

        return cultureNames;
    }

    private IReadOnlyDictionary<string, string> GetTexts(string resourceName, string cultureName)
    {
        return _resources.TryGetValue(resourceName, out var cultures) &&
               cultures.TryGetValue(cultureName, out var texts)
            ? texts
            : new Dictionary<string, string>();
    }

    private static string GetResourceName(string resourcePath, string culture)
    {
        var startIndex = resourcePath.IndexOf(LocalizationPath, StringComparison.Ordinal) + LocalizationPath.Length;
        var suffix = $".{culture}.json";
        return resourcePath[startIndex..^suffix.Length];
    }
}
