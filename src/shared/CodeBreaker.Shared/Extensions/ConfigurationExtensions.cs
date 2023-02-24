using CodeBreaker.Shared.Exceptions;
using Microsoft.Extensions.Configuration;

namespace CodeBreaker.Shared.Extensions;
public static class ConfigurationExtensions
{
    public static string GetRequired(this IConfiguration config, string key) =>
        config[key] ?? throw new ConfigurationNotFoundException(key);
}
