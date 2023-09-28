using CodeBreaker.Services.Authentication.Definitions;
using CodeBreaker.Services.EventArguments;
using Microsoft.Identity.Client;

namespace CodeBreaker.Services.Authentication;

public interface IAuthService
{
    event EventHandler<OnAuthenticationStateChangedEventArgs>? OnAuthenticationStateChanged;

    UserInformation? LastUserInformation { get; }

    bool IsAuthenticated { get; }

    Task RegisterPersistentTokenCacheAsync();

    Task<bool> TryAquireTokenSilentlyAsync(IAuthDefinition authDefinition, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> AquireTokenAsync(IAuthDefinition authHandler, CancellationToken cancellation = default);

    Task LogoutAsync(CancellationToken cancellationToken = default);
}
