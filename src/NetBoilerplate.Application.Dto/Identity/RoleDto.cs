namespace NetBoilerplate.Application.Dto.Identity;

public class RoleDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<string> Permissions { get; set; }
}
