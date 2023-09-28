namespace CodeBreaker.Shared.Exceptions;

public class GameNotFoundException : NotFoundException
{
    public GameNotFoundException()
    {
    }

    public GameNotFoundException(string? message) : base(message)
    {
    }

    public GameNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
