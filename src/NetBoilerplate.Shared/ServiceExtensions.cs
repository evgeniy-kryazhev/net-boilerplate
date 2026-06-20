using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using NetBoilerplate.Shared.Localization;

namespace NetBoilerplate.Shared;

public static class ServiceExtensions
{
    public static IServiceCollection AddNetBoilerplateLocalization(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<JsonLocalizationStore>(_ =>
            new JsonLocalizationStore(typeof(NetBoilerplateResource).Assembly));
        services.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory, JsonStringLocalizerFactory>());

        return services;
    }

    public static IServiceCollection AddRepository<TContext, TEntity>(this IServiceCollection services)
        where TEntity : class, IEntity<Guid>
        where TContext : DbContext
    {
        return services
            .AddTransient<IRepository<TEntity>, EfCoreRepository<TContext, TEntity>>()
            .AddTransient<IRepository<TEntity, Guid>, EfCoreRepository<TContext, TEntity>>();
    }

    public static IServiceCollection AddRepository<TContext, TEntity, TRepository>(this IServiceCollection services)
        where TEntity : class, IEntity<Guid>
        where TRepository : EfCoreRepository<TContext, TEntity>
        where TContext : DbContext
    {
        return services
            .AddRepository<TContext, TEntity>()
            .AddTransient<IRepository<TEntity>, TRepository>();
    }

    public static IServiceCollection AddRepository<TContext, TEntity, TIRepository, TRepository>(this IServiceCollection services)
        where TEntity : class, IEntity<Guid>
        where TRepository : class, TIRepository, IRepository<TEntity>
        where TContext : DbContext
        where TIRepository : class
    {
        return services
            .AddRepository<TContext, TEntity>()
            .AddTransient<IRepository<TEntity>, TRepository>()
            .AddTransient<TIRepository, TRepository>();
    }
}
