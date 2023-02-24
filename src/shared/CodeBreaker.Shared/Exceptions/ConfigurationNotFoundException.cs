
using System.Runtime;

namespace CodeBreaker.Shared.Exceptions;

public class ConfigurationNotFoundException : Exception
{
    public ConfigurationNotFoundException(string key)
    {
        ConfigurationKey = key;
    }
    public string ConfigurationKey { get; set; }
}
