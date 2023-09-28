namespace Microsoft.Extensions.Configuration;

public static class ConfigExtensions
{
    public static string GetRequired(this IConfiguration config, string configKey) =>
        config[configKey] ?? throw new InvalidOperationException($"Could not find configuration with key: \"{configKey}\"");
}
