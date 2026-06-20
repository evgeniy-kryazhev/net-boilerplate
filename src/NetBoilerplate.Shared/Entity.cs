namespace NetBoilerplate.Shared;

public class Entity(Guid id) : Entity<Guid>(id);

public class Entity<TId>(TId id) : IEntity<TId>
{
    public TId Id { get; set; } = id;
}

public interface IEntity<TId>
{
    TId Id { get; set; }
}