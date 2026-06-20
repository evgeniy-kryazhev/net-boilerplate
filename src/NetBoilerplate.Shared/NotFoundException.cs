namespace NetBoilerplate.Shared;

public class NotFoundException(string message) : Exception(message)
{
}
