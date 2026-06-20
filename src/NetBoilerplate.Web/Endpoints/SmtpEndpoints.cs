using NetBoilerplate.Web.Identity;
using NetBoilerplate.Domain.Identity;
using NetBoilerplate.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NetBoilerplate.Web.Endpoints;

public static class SmtpEndpoints
{
    public static void MapSmtpEndpoints(this WebApplication app)
    {
        app.MapPost("/api/smtp/test", SendTestEmailAsync)
            .RequireAuthorization()
            .RequirePermission(ApplicationPermissions.Smtp.Test)
            .Produces(StatusCodes.Status200OK)
            .WithTags("SMTP")
            .WithName("TestSmtp")
            .WithSummary("Send an SMTP test email")
            .WithDescription(
                "Sends a test email to the supplied address using the configured SMTP sender. Requires authentication. Use this to verify outgoing email configuration.");
    }

    private static async Task<Ok> SendTestEmailAsync(
        [FromServices] ISmtpTestSender sender,
        [FromBody] TestSmtpRequest input,
        CancellationToken token)
    {
        await sender.SendTestAsync(input.Email, token);
        return TypedResults.Ok();
    }

    public sealed record TestSmtpRequest(string Email);
}
