namespace NetBoilerplate.Domain.Identity;

public interface ICurrentUser
{
    Guid GetId();
}