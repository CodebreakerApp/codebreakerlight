using Microsoft.Identity.Client;

namespace CodeBreaker.Services.Authentication;

public record UserInformation(
    IAccount Account,
    IEnumerable<string> EmailAddresses,
    string FirstName,
    string LastName,
    string GamerName
)
{
    public static UserInformation FromAuthenticationResult(AuthenticationResult authenticationResult)
    {
        string? GetClaim(string type) => authenticationResult.ClaimsPrincipal.Claims.SingleOrDefault(x => x.Type == type)?.Value;
        return new(
            authenticationResult.Account,
            GetClaim("emails")?.Split(',') ?? throw new MissingClaimException("emails"),
            GetClaim("given_name") ?? throw new MissingClaimException("given_name"),
            GetClaim("family_name") ?? throw new MissingClaimException("family_name"),
            GetClaim("extension_GamerName") ?? throw new MissingClaimException("extension_GamerName")
        );
    }

    public string FullName => $"{FirstName} {LastName}";
}
