using NetBoilerplate.Application.Dto.Identity;
using NetBoilerplate.Application.Services.Identity;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared.Exceptions;
using NetBoilerplate.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NetBoilerplate.Web.Endpoints;

public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var permissions = app.MapGroup("/api/permissions")
            .WithTags("Permissions")
            .RequireAuthorization();

        permissions.MapGet("", async ([FromServices] IRolePermissionService service) =>
                TypedResults.Ok(await service.GetPermissionDefinitionsAsync()))
            .Produces<List<PermissionDefinitionDto>>()
            .WithName("GetPermissionDefinitions")
            .WithSummary("List permission definitions")
            .WithDescription(
                "Returns the server-side permission catalog grouped for an administration UI or AI agent. Each definition has a stable name, display name, group, and optional parent. Requires authentication.");

        var roles = app.MapGroup("/api/roles")
            .WithTags("Roles");

        roles.MapGet("", async (
                    [FromServices] IRolePermissionService service,
                    CancellationToken token) =>
                TypedResults.Ok(await service.GetRolesAsync(token)))
            .RequirePermission(ApplicationPermissions.Identity.Roles)
            .Produces<List<RoleDto>>()
            .WithName("GetRoles")
            .WithSummary("List roles and their permissions")
            .WithDescription(
                $"Returns Identity roles together with granted permissions. Requires `{ApplicationPermissions.Identity.Roles}`.");

        roles.MapPost("", CreateRoleAsync)
            .RequirePermission(ApplicationPermissions.Identity.CreateRoles)
            .Produces<RoleDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .WithName("CreateRole")
            .WithSummary("Create a role")
            .WithDescription(
                $"Creates an Identity role with no permissions. Grant permissions with the role-permissions endpoint. Requires `{ApplicationPermissions.Identity.CreateRoles}`.");

        roles.MapGet("{roleId:guid}/permissions", GetRolePermissionsAsync)
            .RequirePermission(ApplicationPermissions.Identity.ManageRolePermissions)
            .Produces<RoleDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetRolePermissions")
            .WithSummary("Get permissions granted to a role")
            .WithDescription(
                $"Returns one role and its granted permissions. Requires `{ApplicationPermissions.Identity.ManageRolePermissions}`.");

        roles.MapPut("{roleId:guid}/permissions", SetRolePermissionsAsync)
            .RequirePermission(ApplicationPermissions.Identity.ManageRolePermissions)
            .Produces<RoleDto>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("SetRolePermissions")
            .WithSummary("Replace permissions granted to a role")
            .WithDescription(
                $"Replaces all permissions granted to a role. Send stable names from the permission catalog. Requires `{ApplicationPermissions.Identity.ManageRolePermissions}`.");
    }

    private static async Task<Results<Created<RoleDto>, BadRequest<string>>> CreateRoleAsync(
        [FromServices] IRolePermissionService service,
        [FromBody] CreateRoleDto input)
    {
        try
        {
            var role = await service.CreateRoleAsync(input);
            return TypedResults.Created($"/api/roles/{role.Id}", role);
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }

    private static async Task<Results<Ok<RoleDto>, NotFound>> GetRolePermissionsAsync(
        Guid roleId,
        [FromServices] IRolePermissionService service)
    {
        var role = await service.GetRoleAsync(roleId);
        return role == null ? TypedResults.NotFound() : TypedResults.Ok(role);
    }

    private static async Task<Results<Ok<RoleDto>, NotFound, BadRequest<string>>> SetRolePermissionsAsync(
        Guid roleId,
        [FromServices] IRolePermissionService service,
        [FromBody] UpdateRolePermissionsDto input)
    {
        try
        {
            var role = await service.SetRolePermissionsAsync(roleId, input);
            return role == null ? TypedResults.NotFound() : TypedResults.Ok(role);
        }
        catch (UserFriendlyException exception)
        {
            return TypedResults.BadRequest(exception.Message);
        }
    }
}
