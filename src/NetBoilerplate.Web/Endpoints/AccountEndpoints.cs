using NetBoilerplate.Application;
using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Application.Services.Account;
using NetBoilerplate.Domain.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NetBoilerplate.Web.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountsEndpoints(this WebApplication app)
    {
        var group = app
            .MapGroup("/api/accounts")
            .WithTags("Accounts");

        group.MapGet("/me", GetMeAsync)
            .RequireAuthorization()
            .Produces<UserDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("me")
            .WithSummary("Get the current account")
            .WithDescription(
                "Returns the authenticated user's account profile. Requires a bearer access token. Returns 404 if the authenticated user no longer exists.");

        group.MapPost("/register", async ([FromServices] IAccountService accountService,
                [FromBody] RegisterUserDto inputDto) =>
            {
                var userDto = await accountService.CreateAsync(inputDto);
                return TypedResults.Json(userDto);
            })
            .Produces<UserDto>()
            .WithName("register")
            .WithSummary("Register a local account")
            .WithDescription(
                "Creates a local Identity account from the supplied registration data. Use the authorization login endpoint after registration to obtain bearer tokens.");
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetMeAsync([FromServices] IAccountService accountService,
        [FromServices] ICurrentUser currentUser)
    {
        var user = await accountService.GetAsync(currentUser.GetId());
        return user == null ? TypedResults.NotFound()
            : TypedResults.Ok(user.ToDto());
    }
}
