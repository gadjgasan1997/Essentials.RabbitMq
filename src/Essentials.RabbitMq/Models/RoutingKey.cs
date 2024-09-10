using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Ключ маршрутизации
/// </summary>
internal record RoutingKey
{
    // todo parameterized
    public RoutingKey(string? value)
    {
        Value = string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.FullTrim()!.ToLowerInvariant();
    }
    
    /// <summary>
    /// Ключ маршрутизации
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Признак, что ключ маршрутизации пустой
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}