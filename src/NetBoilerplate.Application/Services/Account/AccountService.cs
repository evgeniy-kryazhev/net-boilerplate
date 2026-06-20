using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace NetBoilerplate.Application.Services.Account;

public class AccountService(
    UserManager<ApplicationUser> userManager,
    IUserStore<ApplicationUser> userStore,
    RoleManager<ApplicationRole> roleManager) : IAccountService
{
    public async Task<ApplicationUser?> GetAsync(Guid? userId)
    {
        if (userId is null)
        {
            return null;
        }

        var user = await userManager.FindByIdAsync(userId.Value.ToString());
        return user;
    }

    public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        return user;
    }

    public async Task<ApplicationUser> CreateAsync(RegisterUserDto dto)
    {
        var user = new ApplicationUser();

        await userStore.SetUserNameAsync(user, dto.UserName, CancellationToken.None);

        var emailStore = (IUserEmailStore<ApplicationUser>)userStore;
        await emailStore.SetEmailAsync(user, dto.Email, CancellationToken.None);

        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            await AssignDefaultRoleAsync(user);
            return user;
        }

        var errors = result.Errors.Select(e => e.Description);
        throw new UserFriendlyException(string.Join(Environment.NewLine, errors));
    }

    public async Task<ApplicationUser> CreateAsync(string email, string? avatar)
    {
        var user = new ApplicationUser
        {
            Avatar = avatar
        };

        await userStore.SetUserNameAsync(user, email, CancellationToken.None);

        var emailStore = (IUserEmailStore<ApplicationUser>)userStore;
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);

        var result = await userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            await AssignDefaultRoleAsync(user);
            return user;
        }

        var errors = result.Errors.Select(e => e.Description);
        throw new UserFriendlyException(string.Join(Environment.NewLine, errors));
    }

    private async Task AssignDefaultRoleAsync(ApplicationUser user)
    {
        var role = await roleManager.FindByNameAsync(ApplicationRoles.User);
        if (role?.Name == null)
        {
            return;
        }

        var result = await userManager.AddToRoleAsync(user, role.Name);
        if (result.Succeeded)
        {
            return;
        }

        throw new UserFriendlyException(string.Join(
            Environment.NewLine,
            result.Errors.Select(error => error.Description)));
    }
}
