using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Ключ маршрутизации
/// </summary>
public record RoutingKey
{
    // todo parameterized
    private RoutingKey(string value)
    {
        Value = value;
        IsEmpty = string.IsNullOrWhiteSpace(value);
    }
    
    /// <summary>
    /// Ключ маршрутизации
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Признак, что ключ маршрутизации пустой
    /// </summary>
    public bool IsEmpty { get; }
    
    /// <summary>
    /// Пустой ключ маршрутизации
    /// </summary>
    public static RoutingKey Empty { get; } = Create(null);
    
    /// <summary>
    /// Создает ключ маршрутизации из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Ключ маршрутизации</returns>
    public static RoutingKey Create(string? value)
    {
        var normalizedValue = string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.FullTrim().ToLowerInvariant();
        
        return new RoutingKey(normalizedValue);
    }
    
    /// <summary>
    /// Создает ключ маршрутизации из строки
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Ключ маршрутизации</returns>
    public static implicit operator RoutingKey(string? value) => Create(value);
    
    /// <summary>
    /// В случае, если левый ключ маршрутизации пустой, возвращает правый
    /// </summary>
    /// <param name="left">Ключ маршрутизации</param>
    /// <param name="right">Ключ маршрутизации</param>
    /// <returns>Ключ маршрутизации</returns>
    public static RoutingKey operator |(RoutingKey left, RoutingKey right) => left.IsEmpty ? right : left;
}