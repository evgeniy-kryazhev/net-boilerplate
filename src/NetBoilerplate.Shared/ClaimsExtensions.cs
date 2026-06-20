using System.Security.Claims;

namespace NetBoilerplate.Shared;

public static class ClaimsExtensions
{
    public static string GetValue(this IEnumerable<Claim> claims, string claimType)
    {
        var claim = claims.FirstOrDefault(x => x.Type == claimType);

        if (claim == null)
        {
            throw new NullReferenceException($"Claim not found {claimType}");
        }

        return claim.Value;
    }
}