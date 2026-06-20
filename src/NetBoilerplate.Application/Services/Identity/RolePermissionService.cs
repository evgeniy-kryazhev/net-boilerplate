using System.Security.Claims;
using NetBoilerplate.Application.Dto.Identity;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetBoilerplate.Application.Services.Identity;

public class RolePermissionService(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager) : IRolePermissionService
{
    public Task<List<PermissionDefinitionDto>> GetPermissionDefinitionsAsync()
    {
        return Task.FromResult(ApplicationPermissions.All
            .Select(permission => new PermissionDefinitionDto
            {
                Name = permission.Name,
                DisplayName = permission.DisplayName,
                GroupName = permission.GroupName,
                ParentName = permission.ParentName
            })
            .ToList());
    }

    public async Task<List<RoleDto>> GetRolesAsync(CancellationToken token)
    {
        var roles = await roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync(token);

        var result = new List<RoleDto>(roles.Count);
        foreach (var role in roles)
        {
            result.Add(await MapAsync(role));
        }

        return result;
    }

    public async Task<RoleDto?> GetRoleAsync(Guid roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        return role == null ? null : await MapAsync(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new UserFriendlyException("Role name is required.");
        }

        var role = new ApplicationRole { Name = input.Name.Trim() };
        EnsureSucceeded(await roleManager.CreateAsync(role));
        return await MapAsync(role);
    }

    public async Task<RoleDto?> SetRolePermissionsAsync(Guid roleId, UpdateRolePermissionsDto input)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            return null;
        }

        var requested = input.Permissions
            .Distinct(StringComparer.Ordinal)
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToList();
        var unknown = requested.Where(permission => !ApplicationPermissions.IsDefined(permission)).ToList();
        if (unknown.Count > 0)
        {
            throw new UserFriendlyException($"Unknown permissions: {string.Join(", ", unknown)}.");
        }

        var existing = (await roleManager.GetClaimsAsync(role))
            .Where(claim => claim.Type == ApplicationClaimTypes.Permission)
            .ToList();

        foreach (var claim in existing.Where(claim => !requested.Contains(claim.Value, StringComparer.Ordinal)))
        {
            EnsureSucceeded(await roleManager.RemoveClaimAsync(role, claim));
        }

        var granted = existing.Select(claim => claim.Value).ToHashSet(StringComparer.Ordinal);
        foreach (var permission in requested.Where(permission => !granted.Contains(permission)))
        {
            EnsureSucceeded(await roleManager.AddClaimAsync(
                role,
                new Claim(ApplicationClaimTypes.Permission, permission)));
        }

        return await MapAsync(role);
    }

    public async Task<UserRolesDto?> GetUserRolesAsync(Guid userId, CancellationToken token)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        var roleNames = await userManager.GetRolesAsync(user);
        var roles = await roleManager.Roles
            .Where(role => role.Name != null && roleNames.Contains(role.Name))
            .OrderBy(role => role.Name)
            .ToListAsync(token);

        return new UserRolesDto
        {
            UserId = user.Id,
            Roles = await MapAsync(roles)
        };
    }

    public async Task<UserRolesDto?> SetUserRolesAsync(
        Guid userId,
        UpdateUserRolesDto input,
        CancellationToken token)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        var roleIds = input.RoleIds.Distinct().ToList();
        var roles = await roleManager.Roles
            .Where(role => roleIds.Contains(role.Id))
            .ToListAsync(token);
        if (roles.Count != roleIds.Count)
        {
            throw new UserFriendlyException("One or more roles do not exist.");
        }

        var requested = roles.Select(role => role.Name!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var current = (await userManager.GetRolesAsync(user)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var removed = current.Except(requested, StringComparer.OrdinalIgnoreCase).ToList();
        var added = requested.Except(current, StringComparer.OrdinalIgnoreCase).ToList();

        if (removed.Count > 0)
        {
            EnsureSucceeded(await userManager.RemoveFromRolesAsync(user, removed));
        }

        if (added.Count > 0)
        {
            EnsureSucceeded(await userManager.AddToRolesAsync(user, added));
        }

        return await GetUserRolesAsync(userId, token);
    }

    private async Task<List<RoleDto>> MapAsync(IEnumerable<ApplicationRole> roles)
    {
        var result = new List<RoleDto>();
        foreach (var role in roles)
        {
            result.Add(await MapAsync(role));
        }

        return result;
    }

    private async Task<RoleDto> MapAsync(ApplicationRole role)
    {
        var permissions = (await roleManager.GetClaimsAsync(role))
            .Where(claim => claim.Type == ApplicationClaimTypes.Permission)
            .Select(claim => claim.Value)
            .OrderBy(permission => permission, StringComparer.Ordinal)
            .ToList();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Permissions = permissions
        };
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new UserFriendlyException(string.Join(
            Environment.NewLine,
            result.Errors.Select(error => error.Description)));
    }
}
