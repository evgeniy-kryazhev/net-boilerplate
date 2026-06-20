namespace NetBoilerplate.Application.Dto.Account;

public class RegisterUserDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}