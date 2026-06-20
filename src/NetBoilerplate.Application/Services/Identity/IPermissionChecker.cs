using System.Security.Claims;

namespace NetBoilerplate.Application.Services.Identity;

public interface IPermissionChecker
{
    Task<bool> IsGrantedAsync(ClaimsPrincipal principal, string permissionName);
}
