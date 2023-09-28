namespace CodeBreaker.Services.Authentication.Definitions;

public interface IAuthDefinition
{
    IEnumerable<string> Claims { get; }
}
