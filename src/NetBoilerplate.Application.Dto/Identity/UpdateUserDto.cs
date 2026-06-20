namespace NetBoilerplate.Application.Dto.Identity;

public class UpdateUserDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? Avatar { get; set; }
}
