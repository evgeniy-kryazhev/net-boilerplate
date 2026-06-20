using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Application.Dto.Identity;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared;
using NetBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetBoilerplate.Application.Services.Identity;

public class UserManagementService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IUserManagementService
{
    public async Task<PagedResultDto<UserDto>> GetUsersAsync(
        PagedInputDto input,
        string? search,
        CancellationToken token)
    {
        var query = userManager.Users;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToUpperInvariant();
            query = query.Where(user =>
                (user.NormalizedUserName != null && user.NormalizedUserName.Contains(normalizedSearch)) ||
                (user.NormalizedEmail != null && user.NormalizedEmail.Contains(normalizedSearch)));
        }

        var totalCount = await query.CountAsync(token);
        var users = await query
            .OrderBy(user => user.UserName)
            .Skip(input.Skip)
            .Take(input.Take)
            .ToListAsync(token);

        return new PagedResultDto<UserDto>(users.Select(user => user.ToDto()).ToList(), totalCount);
    }

    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user?.ToDto();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto input)
    {
        var roles = await GetRolesAsync(input.RoleIds);
        var user = new ApplicationUser
        {
            UserName = input.UserName,
            Email = input.Email,
            Avatar = input.Avatar
        };

        EnsureSucceeded(await userManager.CreateAsync(user, input.Password));
        await AddToRolesAsync(user, roles);
        return user.ToDto();
    }

    public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto input)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        EnsureSucceeded(await userManager.SetUserNameAsync(user, input.UserName));
        EnsureSucceeded(await userManager.SetEmailAsync(user, input.Email));
        user.Avatar = input.Avatar;
        EnsureSucceeded(await userManager.UpdateAsync(user));
        return user.ToDto();
    }

    public async Task<UserDto?> SetUserActiveAsync(Guid userId, SetUserActiveDto input)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return null;
        }

        EnsureSucceeded(await userManager.SetLockoutEnabledAsync(user, true));
        EnsureSucceeded(await userManager.SetLockoutEndDateAsync(
            user,
            input.IsActive ? null : DateTimeOffset.MaxValue));
        return user.ToDto();
    }

    public async Task<bool> SetUserPasswordAsync(Guid userId, SetUserPasswordDto input)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        EnsureSucceeded(await userManager.ResetPasswordAsync(user, resetToken, input.Password));
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }

        EnsureSucceeded(await userManager.DeleteAsync(user));
        return true;
    }

    private async Task<List<ApplicationRole>> GetRolesAsync(List<Guid>? roleIds)
    {
        if (roleIds is not { Count: > 0 })
        {
            var defaultRole = await roleManager.FindByNameAsync(ApplicationRoles.User);
            return defaultRole == null ? [] : [defaultRole];
        }

        var ids = roleIds.Distinct().ToList();
        var roles = await roleManager.Roles
            .Where(role => ids.Contains(role.Id))
            .ToListAsync();
        if (roles.Count != ids.Count)
        {
            throw new UserFriendlyException("One or more roles do not exist.");
        }

        return roles;
    }

    private async Task AddToRolesAsync(ApplicationUser user, List<ApplicationRole> roles)
    {
        var roleNames = roles
            .Select(role => role.Name)
            .Where(name => name != null)
            .Cast<string>()
            .ToList();
        if (roleNames.Count > 0)
        {
            EnsureSucceeded(await userManager.AddToRolesAsync(user, roleNames));
        }
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
