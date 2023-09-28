namespace CodeBreaker.Services.Options;

public class AuthOptions
{
    public string TenantName { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;

    public AuthPolicyOptions Policies { get; set; } = new();
}

public class AuthPolicyOptions
{
    public string SignUpSignInPolicy { get; set; } = string.Empty;

    public string SignInPolicy { get; set; } = string.Empty;

    public string SignUpPolicy { get; set; } = string.Empty;

    public string EditProfilePolicy { get; set; } = string.Empty;

    public string ResetPasswordPolicy { get; set; } = string.Empty;
}
