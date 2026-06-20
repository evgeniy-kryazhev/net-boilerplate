namespace NetBoilerplate.Web.Identity;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "NetBoilerplate";
    public string InvitationUrl { get; set; } = "http://localhost:4200/register";
}
