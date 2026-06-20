namespace NetBoilerplate.Shared.Exceptions;

public class ItemNotFoundException(string name) :
    Exception($"Item not found {name}")
{
}