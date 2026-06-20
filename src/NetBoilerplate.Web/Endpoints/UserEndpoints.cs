using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Application.Dto.Identity;
using NetBoilerplate.Application.Services.Identity;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared;
using NetBoilerplate.Shared.Exceptions;
using NetBoilerplate.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NetBoilerplate.Web.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users")
            .WithTags("Users");

        users.MapGet("", async (
                    [FromServices] IUserManagementService service,
                    [FromQuery] int? skip,
                    [FromQuery] int? take,
                    [FromQuery] string? search,
                    CancellationToken token) =>
                TypedResults.Ok(await service.GetUsersAsync(
                    new PagedInputDto(skip ?? 0, take ?? 50),
                    search,
                    token)))
            .RequirePermission(ApplicationPermissions.Identity.Users)
            .Produces<PagedResultDto<UserDto>>()
            .WithName("GetUsers")
            .WithSummary("List users")
            .WithDescription(
                $"Returns a paged list of users. Use `skip`, `take`, and optional `search` to filter by username or email. Requires `{ApplicationPermissions.Identity.Users}`.");

        users.MapGet("{userId:guid}", GetUserAsync)
            .RequirePermission(ApplicationPermissions.Identity.Users)
            .Produces<UserDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetUser")
            .WithSummary("Get a user by ID")
            .WithDescription(
                $"Returns one user profile by GUID. Requires `{ApplicationPermissions.Identity.Users}`.");

        users.MapPost("", CreateUserAsync)
            .RequirePermission(ApplicationPermissions.Identity.CreateUsers)
            .Produces<UserDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .WithName("CreateUser")
            .WithSummary("Create a user")
            .WithDescription(
                $"Creates a local Identity user. When `roleIds` is omitted or empty, the default `{ApplicationRoles.User}` role is assigned when available. Requires `{ApplicationPermissions.Identity.CreateUsers}`.");

        users.MapPut("{userId:guid}", UpdateUserAsync)
            .RequirePermission(ApplicationPermissions.Identity.UpdateUsers)
            .Produces<UserDto>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdateUser")
            .WithSummary("Update a user")
            .WithDescription(
                $"Updates a user's username, email, and optional avatar. Requires `{ApplicationPermissions.Identity.UpdateUsers}`.");

        users.MapPut("{userId:guid}/active", SetUserActiveAsync)
            .RequirePermission(ApplicationPermissions.Identity.UpdateUsers)
            .Produces<UserDto>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SetUserActive")
            .WithSummary("Activate or block a user")
            .WithDescription(
                $"Sets whether a user can log in. Blocking uses the ASP.NET Identity lockout mechanism. Requires `{ApplicationPermissions.Identity.UpdateUsers}`.");

        users.MapPut("{userId:guid}/password", SetUserPasswordAsync)
            .RequirePermission(ApplicationPermissions.Identity.ManageUserPassword)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SetUserPassword")
            .WithSummary("Set a user's password")
            .WithDescription(
                $"Replaces a local user's password using an Identity reset token generated on the server. Requires `{ApplicationPermissions.Identity.ManageUserPassword}`.");

        users.MapDelete("{userId:guid}", DeleteUserAsync)
            .RequirePermission(ApplicationPermissions.Identity.DeleteUsers)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .WithDescription(
                $"Deletes a local Identity user. This is a destructive operation. Requires `{ApplicationPermissions.Identity.DeleteUsers}`.");

        users.MapGet("{userId:guid}/roles", GetUserRolesAsync)
            .RequirePermission(ApplicationPermissions.Identity.ManageUserRoles)
            .Produces<UserRolesDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetUserRoles")
            .WithSummary("Get roles assigned to a user")
            .WithDescription(
                $"Returns Identity roles assigned to a user. Requires `{ApplicationPermissions.Identity.ManageUserRoles}`.");

        users.MapPut("{userId:guid}/roles", SetUserRolesAsync)
            .RequirePermission(ApplicationPermissions.Identity.ManageUserRoles)
            .Produces<UserRolesDto>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SetUserRoles")
            .WithSummary("Replace roles assigned to a user")
            .WithDescription(
                $"Replaces all Identity roles assigned to a user. Use role IDs from the roles endpoint. Requires `{ApplicationPermissions.Identity.ManageUserRoles}`.");
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetUserAsync(
        Guid userId,
        [FromServices] IUserManagementService service)
    {
        var result = await service.GetUserAsync(userId);
        return result == null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Created<UserDto>, BadRequest<string>>> CreateUserAsync(
        [FromServices] IUserManagementService service,
        [FromBody] CreateUserDto input)
    {
        try
        {
            var result = await service.CreateUserAsync(input);
            return TypedResults.Created($"/api/users/{result.Id}", result);
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static async Task<Results<Ok<UserDto>, NotFound, BadRequest<string>>> UpdateUserAsync(
        Guid userId,
        [FromServices] IUserManagementService service,
        [FromBody] UpdateUserDto input)
    {
        try
        {
            var result = await service.UpdateUserAsync(userId, input);
            return result == null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static async Task<Results<Ok<UserDto>, NotFound, BadRequest<string>>> SetUserActiveAsync(
        Guid userId,
        [FromServices] IUserManagementService service,
        [FromBody] SetUserActiveDto input)
    {
        try
        {
            var result = await service.SetUserActiveAsync(userId, input);
            return result == null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<string>>> SetUserPasswordAsync(
        Guid userId,
        [FromServices] IUserManagementService service,
        [FromBody] SetUserPasswordDto input)
    {
        try
        {
            return await service.SetUserPasswordAsync(userId, input)
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<string>>> DeleteUserAsync(
        Guid userId,
        [FromServices] IUserManagementService service)
    {
        try
        {
            return await service.DeleteUserAsync(userId)
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static async Task<Results<Ok<UserRolesDto>, NotFound>> GetUserRolesAsync(
        Guid userId,
        [FromServices] IRolePermissionService service,
        CancellationToken token)
    {
        var result = await service.GetUserRolesAsync(userId, token);
        return result == null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserRolesDto>, NotFound, BadRequest<string>>> SetUserRolesAsync(
        Guid userId,
        [FromServices] IRolePermissionService service,
        [FromBody] UpdateUserRolesDto input,
        CancellationToken token)
    {
        try
        {
            var result = await service.SetUserRolesAsync(userId, input, token);
            return result == null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }
}