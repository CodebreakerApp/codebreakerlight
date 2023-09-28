namespace CodeBreaker.Services.Authentication;

public class MissingClaimException : Exception
{
    public MissingClaimException(string? missingClaim)
    {
        MissingClaim = missingClaim;
    }

    public string? MissingClaim { get; init; }
}
