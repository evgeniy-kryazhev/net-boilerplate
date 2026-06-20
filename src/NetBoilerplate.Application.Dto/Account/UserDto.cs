namespace NetBoilerplate.Application.Dto.Account;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public string? Avatar { get; set; }
    public bool IsActive { get; set; }
}