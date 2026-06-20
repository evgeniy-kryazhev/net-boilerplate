using NetBoilerplate.Application.Dto;
using NetBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;

namespace NetBoilerplate.Application.Services;

public interface IBasicService<TEntity, TDto>
    where TEntity : Entity
    where TDto : EntityDto
{
    Task<TDto?> GetAsync(Guid id, CancellationToken token = default);
    Task<PagedResultDto<TDto>> GetListAsync(PagedInputDto input, CancellationToken token = default);
    Task DeleteAsync(Guid id, CancellationToken token = default);
}

public abstract class BasicService<TEntity, TDto>(IRepository<TEntity> repository) :
    IBasicService<TEntity, TDto>
    where TEntity : Entity
    where TDto : EntityDto
{
    protected IRepository<TEntity> Repository { get; set; } = repository;

    protected abstract TDto Map(TEntity entity);

    public virtual async Task<TDto?> GetAsync(Guid id, CancellationToken token = default)
    {
        var item = await Repository.GetAsync(id);

        return item == null ? null : Map(item);
    }

    public virtual async Task<PagedResultDto<TDto>> GetListAsync(PagedInputDto input, CancellationToken token = default)
    {
        var totalCount = await Repository.GetCountAsync();
        var query = Repository.Query(true);

        var items = await query
            .Order()
            .Skip(input.Skip)
            .Take(input.Take)
            .Distinct()
            .ToListAsync(cancellationToken: token);

        var dtos = items.Select(Map).ToList();

        return new PagedResultDto<TDto>(dtos, totalCount);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        var item = await Repository.GetAsync(id);
        if (item != null) await Repository.DeleteAsync(item);
    }
}