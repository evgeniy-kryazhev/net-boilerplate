namespace NetBoilerplate.Migrator;

public interface IDataSeeder
{
    Task MigrateAsync(CancellationToken cancellationToken);
}