using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Domain.Identity;

namespace NetBoilerplate.Application.Services.Account;

public interface IAccountService
{
    Task<ApplicationUser?> GetAsync(Guid? userId);
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<ApplicationUser> CreateAsync(RegisterUserDto dto);
    Task<ApplicationUser> CreateAsync(string email, string? avatar);
}