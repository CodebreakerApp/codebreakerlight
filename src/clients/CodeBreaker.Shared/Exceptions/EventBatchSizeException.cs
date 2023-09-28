using System.Runtime.Serialization;

namespace CodeBreaker.Shared.Exceptions;

public class EventBatchSizeException : Exception
{
    public EventBatchSizeException()
    {
    }

    public EventBatchSizeException(string? message) : base(message)
    {
    }

    public EventBatchSizeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected EventBatchSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
