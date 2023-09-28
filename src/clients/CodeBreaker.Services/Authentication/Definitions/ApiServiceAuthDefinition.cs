namespace CodeBreaker.Services.Authentication.Definitions;

public class ApiServiceAuthDefinition : IAuthDefinition
{
    public IEnumerable<string> Claims { get; private init; } = new[] {
        $"https://codebreaker3000.onmicrosoft.com/39a665a4-54ce-44cd-b581-0f654a31dbcf/Games.Play",
        $"https://codebreaker3000.onmicrosoft.com/39a665a4-54ce-44cd-b581-0f654a31dbcf/Reports.Read",
    };
}
