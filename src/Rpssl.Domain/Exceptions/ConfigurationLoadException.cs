namespace Rpssl.Domain.Exceptions;

public class ConfigurationLoadException(string configuration) : Exception("Failed to load configuration: " + configuration)
{
}
