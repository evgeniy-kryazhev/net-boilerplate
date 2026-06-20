using NetBoilerplate.Shared;
using Microsoft.AspNetCore.Identity;

namespace NetBoilerplate.Domain.Identity;

public class ApplicationUser : IdentityUser<Guid>, IEntity<Guid>
{
    public string? Avatar { get; set; }
}