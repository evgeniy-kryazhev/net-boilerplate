namespace NetBoilerplate.Application.Dto;

public class EntityDto(Guid id)
{
    public Guid Id { get; init; } = id;
}