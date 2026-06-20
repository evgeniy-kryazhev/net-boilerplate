namespace NetBoilerplate.Application.Services.Passwords;

public interface IPasswordGenerator
{
    string Generate(int length = 16);
}

public class PasswordGenerator : IPasswordGenerator
{
    public string Generate(int length = 16)
    {
        var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
        return new string(Enumerable.Repeat(characters, length)
            .Select(s => s[System.Security.Cryptography.RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
    }
}
