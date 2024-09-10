namespace Essentials.RabbitMq.Exceptions;

/// <summary>
/// Исключение о неверной конфигурации
/// </summary>
public class InvalidConfigurationException : Exception
{
    internal InvalidConfigurationException(string message)
        : base(message)
    { }
    
    internal InvalidConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}