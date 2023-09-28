using CodeBreaker.Services.Authentication.Definitions;
using CodeBreaker.Services.EventArguments;
using CodeBreaker.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace CodeBreaker.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly AuthOptions _authOptions;

    private string TenantName => _authOptions.TenantName;

    private string Tenant => $"{TenantName}.onmicrosoft.com";

    private string AzureAdB2CHostname => $"{TenantName}.b2clogin.com";

    private string RedirectUri => _authOptions.RedirectUri;

    private string AuthorityBase => $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";

    private string AuthoritySignUpSignIn => $"{AuthorityBase}{_authOptions.Policies.SignUpSignInPolicy}";

    private string AuthoritySignUp=> $"{AuthorityBase}{_authOptions.Policies.SignUpPolicy}";

    private string AuthoritySignIn => $"{AuthorityBase}{_authOptions.Policies.SignInPolicy}";

    private string AuthorityEditProfile => $"{AuthorityBase}{_authOptions.Policies.EditProfilePolicy}";

    private string AuthorityResetPassword => $"{AuthorityBase}{_authOptions.Policies.ResetPasswordPolicy}";

    private readonly static string PersistentTokenCacheDirectory = Path.Combine(MsalCacheHelper.UserRootDirectory, ".codebreaker");

    private readonly static string PersistentTokenCacheFileName = "codebreaker_token_cache.txt";

    private readonly ILogger _logger;

    private readonly IPublicClientApplication _publicClientApplication;

    private UserInformation? _lastUserInformation;

    public event EventHandler<OnAuthenticationStateChangedEventArgs>? OnAuthenticationStateChanged;

    public AuthService(ILogger<AuthService> logger, IOptions<AuthOptions> authOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authOptions = authOptions.Value;
        _publicClientApplication = PublicClientApplicationBuilder.Create(_authOptions.ClientId)
            .WithB2CAuthority(AuthoritySignUpSignIn)
            .WithRedirectUri(RedirectUri)
            //.WithLogging(Log, LogLevel.Info, false) // don't log P(ersonally) I(dentifiable) I(nformation) details on a regular basis
            .Build();
    }

    public async Task RegisterPersistentTokenCacheAsync()
    {
        var storageProperties = new StorageCreationPropertiesBuilder(PersistentTokenCacheFileName, PersistentTokenCacheDirectory).Build();
        var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
        cacheHelper.RegisterCache(_publicClientApplication.UserTokenCache);
    }

    public UserInformation? LastUserInformation
    {
        get => _lastUserInformation;
        private set
        {
            _lastUserInformation = value;
            OnAuthenticationStateChanged?.Invoke(this, new());
        }
    }

    public bool IsAuthenticated => LastUserInformation is not null;

    public async Task<bool> TryAquireTokenSilentlyAsync(IAuthDefinition authDefinition, CancellationToken cancellationToken = default)
    {
        IAccount? account = (await GetAccountsAsync(cancellationToken)).FirstOrDefault();

        try
        {
            AuthenticationResult result = await _publicClientApplication.AcquireTokenSilent(authDefinition.Claims, account).ExecuteAsync(cancellationToken);
            LastUserInformation = UserInformation.FromAuthenticationResult(result);
            return true;
        }
        catch (MsalUiRequiredException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to aquire token silently.");
            throw;
        }
    }

    public async Task<AuthenticationResult> AquireTokenAsync(IAuthDefinition authDefinition, CancellationToken cancellationToken = default)
    {
        IAccount? account = (await GetAccountsAsync(cancellationToken)).FirstOrDefault();

        try
        {
            AuthenticationResult result = await _publicClientApplication.AcquireTokenSilent(authDefinition.Claims, account).ExecuteAsync(cancellationToken);
            LastUserInformation = UserInformation.FromAuthenticationResult(result);
            return result;
        }
        catch (MsalUiRequiredException)
        {
            _logger.LogInformation("Unable to aquire token silently. Continueing interactively...");

            try
            {
                AuthenticationResult result = await _publicClientApplication
                    .AcquireTokenInteractive(authDefinition.Claims)
                    .ExecuteAsync(cancellationToken);
                LastUserInformation = UserInformation.FromAuthenticationResult(result);
                return result;
            }
            catch (MsalException msalEx)
            {
                _logger.LogError(msalEx, "Unable to aquire token interactively.");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to aquire token silently.");
            throw;
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await GetAccountsAsync(cancellationToken);

        foreach (var account in accounts)
            await _publicClientApplication.RemoveAsync(account);

        LastUserInformation = null;
    }

    private Task<IEnumerable<IAccount>> GetAccountsAsync(CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return _publicClientApplication.GetAccountsAsync(_authOptions.Policies.SignUpSignInPolicy);
    }
}
