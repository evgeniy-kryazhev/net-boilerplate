using NetBoilerplate.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NetBoilerplate.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    private const int DefaultStringMaxLength = 4000;
    private const int IdentityKeyMaxLength = 128;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.Property(login => login.LoginProvider).HasMaxLength(IdentityKeyMaxLength);
            entity.Property(login => login.ProviderKey).HasMaxLength(IdentityKeyMaxLength);
        });

        builder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.Property(token => token.LoginProvider).HasMaxLength(IdentityKeyMaxLength);
            entity.Property(token => token.Name).HasMaxLength(IdentityKeyMaxLength);
        });
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .HaveMaxLength(DefaultStringMaxLength);
    }
}
