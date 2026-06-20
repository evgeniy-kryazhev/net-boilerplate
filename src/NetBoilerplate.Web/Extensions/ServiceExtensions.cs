using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Web.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace NetBoilerplate.Web.Extensions;

public static class ServiceExtensions
{
    public static void AddConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICurrentUser, CurrentUser>();
        services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();
        services.AddTransient<ISmtpTestSender, EmailSender>();
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
    }

    public static void AddAuth(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireUppercase = false;

            options.ClaimsIdentity.UserNameClaimType = "name";
            options.ClaimsIdentity.UserIdClaimType = "sub";
            options.ClaimsIdentity.EmailClaimType = "email";
            options.ClaimsIdentity.RoleClaimType = "roles";
            options.ClaimsIdentity.SecurityStampClaimType = "security_stamp";
        });

        services
            .AddAuthentication(IdentityConstants.BearerScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorization();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
}
