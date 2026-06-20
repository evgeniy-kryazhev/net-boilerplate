using Microsoft.EntityFrameworkCore;

namespace NetBoilerplate.Shared;

public class EfCoreRepository<TContext, TEntity>(
    TContext context) :
    IRepository<TEntity>
    where TEntity : class, IEntity<Guid>
    where TContext : DbContext
{
    public async Task InsertAsync(TEntity entity)
    {
        await context.Set<TEntity>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<TEntity?> GetAsync(Guid id, bool includeDetails = false)
    {
        var entity = await Query(includeDetails)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        return entity;
    }

    public async Task<List<TEntity>> GetListAsync(bool includeDetails = false)
    {
        var query = Query(includeDetails);

        return await query
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await Query()
            .CountAsync();
    }

    public IQueryable<TEntity> Query(bool includeDetails = false)
    {
        return context.Set<TEntity>();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        var set = context.Set<TEntity>();
        set.Attach(entity);
        set.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        context.Attach(entity);
        var entry = context.Entry(entity);

        entry.CurrentValues.SetValues(entity);

        var result = context.Update(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }
}