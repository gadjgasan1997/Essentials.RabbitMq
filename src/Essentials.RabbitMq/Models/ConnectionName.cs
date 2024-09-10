using Essentials.Utils.Extensions;
using static Essentials.RabbitMq.Subscriber.Models.SubscriptionKey;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Название соединения
/// </summary>
public record ConnectionName
{
    private ConnectionName(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Название соединения
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Создает название соединения из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Название соединения</returns>
    public static ConnectionName Create(string value)
    {
        value.CheckNotNullOrEmpty("Название соединения не может быть пустым");
        
        var normalizedValue = value.FullTrim().ToLowerInvariant();
        if (normalizedValue.Contains(DELIMITER))
        {
            throw new FormatException(
                $"Название соединения '{value}' не должно содержать следующие символы: '{DELIMITER}'");
        }
        
        return new ConnectionName(normalizedValue);
    }
    
    /// <summary>
    /// Создает название соединения из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Название соединения</returns>
    public static implicit operator ConnectionName(string value) => Create(value);
}