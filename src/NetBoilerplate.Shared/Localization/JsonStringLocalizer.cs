using System.Globalization;
using Microsoft.Extensions.Localization;

namespace NetBoilerplate.Shared.Localization;

internal sealed class JsonStringLocalizer(JsonLocalizationStore store, string resourceName) : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            var value = store.GetString(resourceName, name, CultureInfo.CurrentUICulture);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localizedString = this[name];
            return new LocalizedString(name,
                string.Format(CultureInfo.CurrentCulture, localizedString.Value, arguments),
                localizedString.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return store.GetAllStrings(resourceName, CultureInfo.CurrentUICulture, includeParentCultures)
            .Select(pair => new LocalizedString(pair.Key, pair.Value));
    }
}
