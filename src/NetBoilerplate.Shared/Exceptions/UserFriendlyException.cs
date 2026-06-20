namespace NetBoilerplate.Shared.Exceptions;

public class UserFriendlyException(string message) : Exception(message);