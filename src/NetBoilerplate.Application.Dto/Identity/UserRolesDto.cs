namespace NetBoilerplate.Application.Dto.Identity;

public class UserRolesDto
{
    public required Guid UserId { get; set; }
    public required List<RoleDto> Roles { get; set; }
}
