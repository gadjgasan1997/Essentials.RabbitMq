using Essentials.Utils.Extensions;
using static Essentials.RabbitMq.Subscriber.Models.SubscriptionKey;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Название очереди
/// </summary>
public record QueueName
{
    // todo parameterized
    private QueueName(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Название очереди
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Создает название очереди из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Название очереди</returns>
    public static QueueName Create(string value)
    {
        value.CheckNotNullOrEmpty("Название очереди не может быть пустым");
        
        var normalizedValue = value.FullTrim();
        if (normalizedValue.Contains(DELIMITER))
        {
            throw new FormatException(
                $"Название очереди '{value}' не должно содержать следующие символы: '{DELIMITER}'");
        }
        
        return new QueueName(normalizedValue);
    }
    
    /// <summary>
    /// Создает название очереди из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Название очереди</returns>
    public static implicit operator QueueName(string value) => Create(value);
}