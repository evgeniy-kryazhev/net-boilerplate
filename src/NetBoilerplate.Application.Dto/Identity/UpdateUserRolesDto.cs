namespace NetBoilerplate.Application.Dto.Identity;

public class UpdateUserRolesDto
{
    public required List<Guid> RoleIds { get; set; }
}
