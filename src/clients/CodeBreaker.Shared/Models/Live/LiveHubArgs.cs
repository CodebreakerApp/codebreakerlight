namespace CodeBreaker.Shared.Models.Live;

public record LiveHubArgs
{
    private readonly string _name = string.Empty;

    private readonly object _data = new();

    public LiveHubArgs(string name, object data)
    {
        Name = name;
        Data = data;
    }

    public string Name
    {
        get => _name;
        init => _name = value ?? throw new ArgumentNullException(nameof(Name));
    }

    public object Data
    {
        get => _data;
        init => _data = value ?? throw new ArgumentNullException(nameof(Data));
    }
}
