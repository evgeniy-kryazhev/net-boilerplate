namespace NetBoilerplate.Shared;

public class EventInsertEto<TEntity>(TEntity entity) where TEntity : class
{
    public TEntity Entity { get; init; } = entity;
}