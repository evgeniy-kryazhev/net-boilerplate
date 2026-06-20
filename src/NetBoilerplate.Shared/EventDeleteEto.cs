namespace NetBoilerplate.Shared;

public class EventDeleteEto<TEntity>(TEntity entity)
{
    public TEntity Entity { get; init; } = entity;
}