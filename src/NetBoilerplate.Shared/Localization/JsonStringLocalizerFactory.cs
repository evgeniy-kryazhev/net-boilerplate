using Microsoft.Extensions.Localization;

namespace NetBoilerplate.Shared.Localization;

public sealed class JsonStringLocalizerFactory(JsonLocalizationStore store) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
    {
        var resourceName = resourceSource
            .GetCustomAttributes(typeof(LocalizationResourceNameAttribute), false)
            .Cast<LocalizationResourceNameAttribute>()
            .SingleOrDefault()?.Name ?? resourceSource.Name;

        return new JsonStringLocalizer(store, resourceName);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(store, baseName.Split('.').Last());
    }
}
