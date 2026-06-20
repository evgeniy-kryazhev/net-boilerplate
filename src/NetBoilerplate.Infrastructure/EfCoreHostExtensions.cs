using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace NetBoilerplate.Infrastructure;

public static class EfCoreHostExtensions
{
    public static void AddEfCore(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.UseSqlServer(connectionString);
            options.EnableDetailedErrors();
            options.ConfigureWarnings(c => c.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddRepository<ApplicationDbContext, ApplicationUser>();
    }
}
