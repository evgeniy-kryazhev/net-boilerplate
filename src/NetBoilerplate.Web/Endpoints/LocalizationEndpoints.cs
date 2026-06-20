using System.Globalization;
using NetBoilerplate.Shared.Localization;
using Microsoft.Extensions.Localization;

namespace NetBoilerplate.Web.Endpoints;

public static class LocalizationEndpoints
{
    public static void MapLocalizationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/localization", (IStringLocalizer<NetBoilerplateResource> localizer) =>
            {
                var cultures = localizer.GetAllStrings(true);
                var texts = cultures.ToDictionary(text => text.Name, text => text.Value);

                return TypedResults.Ok(new LocalizationResponseDto(
                    CultureInfo.CurrentUICulture.Name,
                    texts));
            })
            .Produces<LocalizationResponseDto>()
            .WithTags("Localization")
            .WithName("GetLocalization")
            .WithSummary("Get localized UI texts")
            .WithDescription(
                "Returns all localization key-value pairs for the request UI culture together with the resolved culture name. Use the Accept-Language header to request a supported culture such as `en` or `ru`.");
    }

    public sealed record LocalizationResponseDto(
        string Culture,
        Dictionary<string, string> Texts);
}
