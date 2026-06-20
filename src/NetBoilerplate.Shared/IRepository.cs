namespace NetBoilerplate.Shared;

public interface IRepository<TEntity> : IRepository<TEntity, Guid>;

public interface IRepository<TEntity, in TId>
{
    Task<TEntity?> GetAsync(TId id, bool includeDetails = false);
    Task<List<TEntity>> GetListAsync(bool includeDetails = false);
    Task<int> GetCountAsync();
    IQueryable<TEntity> Query(bool includeDetails = false);

    Task InsertAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
}