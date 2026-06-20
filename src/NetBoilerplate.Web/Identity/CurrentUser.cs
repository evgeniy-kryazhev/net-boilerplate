using System.Security.Claims;
using System.Security.Principal;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace NetBoilerplate.Web.Identity;

public class CurrentUser(IHttpContextAccessor httpContext,
    IOptions<IdentityOptions> options) : ICurrentUser
{
    public Guid GetId()
    {
        return GetOrNullCurrentUserId() ??
               throw new UnauthorizedAccessException();
    }

    private Guid? GetOrNullCurrentUserId()
    {
        var identity = httpContext.HttpContext?.User.Identity;

        if (identity is not { IsAuthenticated: true })
        {
            return null;
        }

        var identitySub = GetSubjectId(identity, options.Value);
        return identitySub.ToGuid();
    }

    private static string GetSubjectId(IIdentity identity, IdentityOptions options)
    {
        var id = identity as ClaimsIdentity;
        var claim = id?.FindFirst(options.ClaimsIdentity.UserIdClaimType);

        if (claim == null)
        {
            throw new UnauthorizedAccessException("sub claim is missing");
        }

        return claim.Value;
    }
}