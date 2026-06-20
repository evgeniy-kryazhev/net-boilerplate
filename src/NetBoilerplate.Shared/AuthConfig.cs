namespace NetBoilerplate.Shared;

public class AuthConfig
{
    public required string Secret { get; set; }
    public required string Audience { get; set; }
    public required string Authority { get; set; }
}