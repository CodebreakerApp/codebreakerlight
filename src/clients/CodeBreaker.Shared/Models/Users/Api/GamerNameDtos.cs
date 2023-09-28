namespace CodeBreaker.Shared.Models.Users.Api;

public record class GamerNameValidityResponse(bool Valid);

public record class GamerNameSuggestionsResponse(IAsyncEnumerable<string> Suggestions);
