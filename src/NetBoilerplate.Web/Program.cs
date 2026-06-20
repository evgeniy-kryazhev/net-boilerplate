using NetBoilerplate.Application;
using NetBoilerplate.Infrastructure;
using NetBoilerplate.Shared;
using NetBoilerplate.Web;
using NetBoilerplate.Web.Endpoints;
using NetBoilerplate.Web.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: SeriLogCustomThemes.Theme())
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = long.MaxValue; });

    var connectionString = Environment.GetEnvironmentVariable("DB_URL") ??
                           builder.Configuration.GetConnectionString("Default") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddConfigs(builder.Configuration);
    builder.Services.AddNetBoilerplateLocalization();
    builder.Services.AddEfCore(connectionString);
    builder.Services.AddFileStorage(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddAuth();
    builder.Services.AddDataProtection();
    builder.Services.AddSerilog();
    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCaching();
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "en", "ru" };
        options.SetDefaultCulture("en")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
        options.ApplyCurrentCultureToResponseHeaders = true;
    });
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.Name = "XSRF-TOKEN";
    });
    builder.Services.AddHealthChecks();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto |
            ForwardedHeaders.XForwardedHost;
    });

    builder.Services.AddCors(o =>
    {
        o.AddDefaultPolicy(p => p
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );
    });

    builder.Services.AddOpenApi(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Description =
                "API with Identity, role and permission management. " +
                "Most resource operations require bearer authentication and role-granted permissions. " +
                "Use the permissions, roles, and user-role endpoints to inspect and manage access.";
            return Task.CompletedTask;
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRequestLocalization();

    app.UseCors();
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context => { await TypedResults.Problem().ExecuteAsync(context); });
    });
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    app.MapOpenApi();
    app.MapScalarApiReference();

    app
        .MapGet("/", () => TypedResults.Redirect("/scalar"))
        .ExcludeFromDescription();

    app.MapAccountsEndpoints();
    app.MapAuthorizationEndpoints();
    app.MapPermissionEndpoints();
    app.MapUserEndpoints();
    app.MapLocalizationEndpoints();
    app.MapSmtpEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}