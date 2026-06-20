using System.Security.Claims;
using NetBoilerplate.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NetBoilerplate.Migrator.Seeders;

public class IdentityDataSeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IDataSeeder
{
    private const string AdminEmail = "admin@mail.ru";
    private const string AdminPassword = "1q2w3E*";

    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        var adminRole = await EnsureRoleAsync(
            ApplicationRoles.Admin,
            ApplicationPermissions.All.Select(permission => permission.Name));
        var userRole = await EnsureRoleAsync(ApplicationRoles.User, ApplicationPermissions.DefaultUser);

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
            };
            EnsureSucceeded(await userManager.CreateAsync(admin, AdminPassword));
        }

        if (!await userManager.IsInRoleAsync(admin, adminRole.Name!))
        {
            EnsureSucceeded(await userManager.AddToRoleAsync(admin, adminRole.Name!));
        }

        var users = await userManager.Users.ToListAsync(cancellationToken);
        foreach (var user in users.Where(user => user.Id != admin.Id))
        {
            if (!await userManager.IsInRoleAsync(user, userRole.Name!))
            {
                EnsureSucceeded(await userManager.AddToRoleAsync(user, userRole.Name!));
            }
        }
    }

    private async Task<ApplicationRole> EnsureRoleAsync(string name, IEnumerable<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(name);
        if (role == null)
        {
            role = new ApplicationRole { Name = name };
            EnsureSucceeded(await roleManager.CreateAsync(role));
        }

        var granted = (await roleManager.GetClaimsAsync(role))
            .Where(claim => claim.Type == ApplicationClaimTypes.Permission)
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var permission in permissions.Where(permission => !granted.Contains(permission)))
        {
            EnsureSucceeded(await roleManager.AddClaimAsync(
                role,
                new Claim(ApplicationClaimTypes.Permission, permission)));
        }

        return role;
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new InvalidOperationException(string.Join(
            Environment.NewLine,
            result.Errors.Select(error => error.Description)));
    }
}
