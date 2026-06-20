using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Domain.Identity;

namespace NetBoilerplate.Application;

public static class MappingExtensions
{
    public static UserDto ToDto(this ApplicationUser applicationUser)
    {
        return new UserDto
        {
            Id = applicationUser.Id,
            Email = applicationUser.Email ?? string.Empty,
            UserName = applicationUser.UserName ?? string.Empty,
            Avatar = applicationUser.Avatar,
            IsActive = !applicationUser.LockoutEnabled ||
                       applicationUser.LockoutEnd == null ||
                       applicationUser.LockoutEnd <= DateTimeOffset.UtcNow
        };
    }
}
