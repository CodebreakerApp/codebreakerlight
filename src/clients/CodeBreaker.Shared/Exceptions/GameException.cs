namespace CodeBreaker.Shared.Exceptions;


[Serializable]
public class GameException : Exception
{
    public GameException() { }
    public GameException(string message) : base(message) { }
    public GameException(string message, Exception inner) : base(message, inner) { }
    protected GameException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
