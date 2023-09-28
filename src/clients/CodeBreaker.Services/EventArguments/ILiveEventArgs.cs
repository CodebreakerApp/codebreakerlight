namespace CodeBreaker.Services.EventArguments;

public interface ILiveEventArgs<T>
{
    public T Data { get; init; }
}
