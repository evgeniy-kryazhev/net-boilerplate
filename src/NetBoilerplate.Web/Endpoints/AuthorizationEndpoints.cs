using NetBoilerplate.Domain.Identity;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace NetBoilerplate.Web.Endpoints;

public static class AuthorizationEndpoints
{
    public static void MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
    {
        //app.MapIdentityApi<ApplicationUser>().WithTags("Authorization");

        var group = app.MapGroup("/api/authorization")
            .WithTags("Authorization");

        group.MapPost("/login", LoginAsync)
            .Produces<AccessTokenResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("Login")
            .WithSummary("Log in with email and password")
            .WithDescription(
                "Authenticates a local account using email and password. On success, ASP.NET Identity issues bearer authentication data in the response. Returns 401 for invalid credentials.");

        group.MapPost("/login/refresh", RefreshTokenAsync)
            .Produces<AccessTokenResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("RefreshToken")
            .WithSummary("Refresh bearer authentication")
            .WithDescription(
                "Exchanges a valid, unexpired refresh token for renewed bearer authentication data. Use this when the access token has expired. Returns an authentication challenge for invalid, expired, or revoked refresh tokens.");
    }

    private static async
        Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>>
        RefreshTokenAsync([FromBody] RefreshRequest refreshRequest,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] IOptionsMonitor<BearerTokenOptions> bearerTokenOptions, TimeProvider timeProvider)
    {
        var refreshTokenProtector =
            bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

    private static async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, UnauthorizedHttpResult>> LoginAsync(
        [FromBody] LoginRequest login,
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] SignInManager<ApplicationUser> signInManager)
    {
        signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

        var user = await userManager.FindByEmailAsync(login.Email);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
        if (!result.Succeeded)
        {
            return TypedResults.Unauthorized();
        }

        await signInManager.SignInAsync(user, true);
        return TypedResults.Empty;
    }
}
