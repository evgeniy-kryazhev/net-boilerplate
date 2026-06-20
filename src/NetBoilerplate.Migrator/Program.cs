using NetBoilerplate.Infrastructure;
using NetBoilerplate.Migrator;
using NetBoilerplate.Migrator.Seeders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Fatal)
    .CreateLogger();

try
{

    var builder = Host.CreateApplicationBuilder(args);
    
    builder.Services.AddSerilog();

    builder.Configuration
        .AddJsonFile("appsettings.json", false, false)
        .Build();

    var connectionString = Environment.GetEnvironmentVariable("DB_URL") ??
                           builder.Configuration.GetConnectionString("Default") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDataProtection();
    builder.Services.AddEfCore(connectionString);
    
    builder.Services.AddTransient<IDataSeeder, IdentityDataSeeder>();

    builder.Services.AddHostedService<DatabaseMigration>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
