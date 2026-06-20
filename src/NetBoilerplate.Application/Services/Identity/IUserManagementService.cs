using NetBoilerplate.Application.Dto.Account;
using NetBoilerplate.Application.Dto.Identity;
using NetBoilerplate.Shared;

namespace NetBoilerplate.Application.Services.Identity;

public interface IUserManagementService
{
    Task<PagedResultDto<UserDto>> GetUsersAsync(
        PagedInputDto input,
        string? search,
        CancellationToken token);

    Task<UserDto?> GetUserAsync(Guid userId);
    Task<UserDto> CreateUserAsync(CreateUserDto input);
    Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto input);
    Task<UserDto?> SetUserActiveAsync(Guid userId, SetUserActiveDto input);
    Task<bool> SetUserPasswordAsync(Guid userId, SetUserPasswordDto input);
    Task<bool> DeleteUserAsync(Guid userId);
}
