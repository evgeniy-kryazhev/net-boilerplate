using NetBoilerplate.Web.Identity;

namespace NetBoilerplate.Web.Extensions;

public static class AuthorizationEndpointExtensions
{
    public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, string permissionName)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization(policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement(permissionName));
        });
    }
}
