namespace NetBoilerplate.Web.Identity;

public interface ISmtpTestSender
{
    Task SendTestAsync(string email, CancellationToken token = default);
}
