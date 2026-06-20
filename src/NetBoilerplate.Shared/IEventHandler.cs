namespace NetBoilerplate.Shared;

public interface IEventHandler<in TArgs> where TArgs : class
{
    Task Execute(TArgs args);
}