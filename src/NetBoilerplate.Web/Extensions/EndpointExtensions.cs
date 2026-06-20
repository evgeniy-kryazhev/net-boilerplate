using NetBoilerplate.Application.Dto;
using NetBoilerplate.Application.Services;
using NetBoilerplate.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace NetBoilerplate.Web.Extensions;

public static class EndpointExtensions
{
    public static RouteGroupBuilder MapEndpoints<TEntity, TDto>(
        this WebApplication app,
        PathString path,
        string tag,
        string listPermission,
        string getPermission)
        where TEntity : Entity
        where TDto : EntityDto
    {
        var group = app
            .MapGroup(path)
            .WithTags(tag);

        group.MapGet("", async (
                [FromServices] IBasicService<TEntity, TDto> service,
                [FromQuery] int? skip, [FromQuery] int? take, CancellationToken token) =>
            {
                var items = await service.GetListAsync(new PagedInputDto(skip ?? 0, take ?? 50), token);
                return TypedResults.Ok(items);
            })
            .RequirePermission(listPermission)
            .Produces<PagedResultDto<TDto>>()
            .WithName($"Get{tag}")
            .WithSummary($"List {tag.ToLowerInvariant()}")
            .WithDescription(
                $"Returns a paged list of {tag.ToLowerInvariant()}. Use `skip` for the zero-based offset and `take` for the maximum number of items. " +
                "If omitted, `skip` defaults to 0 and `take` defaults to 50.");

        group.MapGet("{id:guid}", GetItemByIdAsync<TEntity, TDto>)
            .RequirePermission(getPermission)
            .Produces<TDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName($"Get{tag}ById")
            .WithSummary($"Get a {tag.TrimEnd('s').ToLowerInvariant()} by ID")
            .WithDescription(
                $"Returns one {tag.TrimEnd('s').ToLowerInvariant()} by its GUID. Returns 404 when the item does not exist or is not accessible to the current user.");

        return group;
    }

    private static async Task<Results<Ok<TDto>, NotFound>> GetItemByIdAsync<TEntity, TDto>(Guid id,
        IBasicService<TEntity, TDto> service)
        where TEntity : Entity
        where TDto : EntityDto
    {
        var result = await service.GetAsync(id);

        if (result == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }
}
