namespace CodeBreaker.Services.Authentication.Definitions;

public class LiveServiceAuthDefinition : IAuthDefinition
{
    public IEnumerable<string> Claims { get; private init; } = new[] {
        $"https://codebreaker3000.onmicrosoft.com/77d424b0-f92c-4f00-bd88-c6645f0d11e7/Messages.ReadFromAllGroups",
        $"https://codebreaker3000.onmicrosoft.com/77d424b0-f92c-4f00-bd88-c6645f0d11e7/Messages.ReadFromSameGroup",
    };
}
