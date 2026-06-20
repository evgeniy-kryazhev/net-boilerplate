using NetBoilerplate.Application.Services.Account;
using NetBoilerplate.Application.Services.Identity;
using NetBoilerplate.Application.Services.Passwords;
using Microsoft.Extensions.DependencyInjection;

namespace NetBoilerplate.Application;

public static class ApplicationHostExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IPermissionChecker, PermissionChecker>();
        services.AddTransient<IRolePermissionService, RolePermissionService>();
        services.AddTransient<IUserManagementService, UserManagementService>();
        services.AddTransient<IPasswordGenerator, PasswordGenerator>();
    }
}
