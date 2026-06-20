namespace NetBoilerplate.Application.Dto.Identity;

public class PermissionDefinitionDto
{
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required string GroupName { get; set; }
    public string? ParentName { get; set; }
}
