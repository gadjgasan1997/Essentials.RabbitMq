using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Название обменника
/// </summary>
public record ExchangeName
{
    private ExchangeName(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// Название обменника
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Создает название обменника из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Название обменника</returns>
    public static ExchangeName Create(string value)
    {
        value.CheckNotNullOrEmpty("Название обменника не может быть пустым");
        
        var normalizedValue = value.FullTrim().ToLowerInvariant();
        return new ExchangeName(normalizedValue);
    }
    
    /// <summary>
    /// Создает название обменника из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Название обменника</returns>
    public static implicit operator ExchangeName(string value) => Create(value);
}