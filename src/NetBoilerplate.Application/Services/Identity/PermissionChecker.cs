using System.Security.Claims;
using NetBoilerplate.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace NetBoilerplate.Application.Services.Identity;

public class PermissionChecker(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> identityOptions) : IPermissionChecker
{
    public async Task<bool> IsGrantedAsync(ClaimsPrincipal principal, string permissionName)
    {
        if (!ApplicationPermissions.IsDefined(permissionName) || principal.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var claimType = identityOptions.Value.ClaimsIdentity.UserIdClaimType;
        var userId = principal.FindFirstValue(claimType);
        if (userId == null)
        {
            return false;
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        foreach (var roleName in await userManager.GetRolesAsync(user))
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                continue;
            }

            var claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(claim =>
                    claim.Type == ApplicationClaimTypes.Permission &&
                    claim.Value == permissionName))
            {
                return true;
            }
        }

        return false;
    }
}
