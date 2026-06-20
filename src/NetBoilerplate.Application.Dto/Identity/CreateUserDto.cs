namespace NetBoilerplate.Application.Dto.Identity;

public class CreateUserDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? Avatar { get; set; }
    public List<Guid>? RoleIds { get; set; }
}
