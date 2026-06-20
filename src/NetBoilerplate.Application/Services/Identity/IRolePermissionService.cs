using NetBoilerplate.Application.Dto.Identity;

namespace NetBoilerplate.Application.Services.Identity;

public interface IRolePermissionService
{
    Task<List<PermissionDefinitionDto>> GetPermissionDefinitionsAsync();
    Task<List<RoleDto>> GetRolesAsync(CancellationToken token);
    Task<RoleDto?> GetRoleAsync(Guid roleId);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto input);
    Task<RoleDto?> SetRolePermissionsAsync(Guid roleId, UpdateRolePermissionsDto input);
    Task<UserRolesDto?> GetUserRolesAsync(Guid userId, CancellationToken token);
    Task<UserRolesDto?> SetUserRolesAsync(Guid userId, UpdateUserRolesDto input, CancellationToken token);
}
