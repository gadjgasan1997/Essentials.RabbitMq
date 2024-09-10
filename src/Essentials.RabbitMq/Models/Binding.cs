// ReSharper disable UsageOfDefaultStructEquality
namespace Essentials.RabbitMq.Models;

/// <summary>
/// Привязка
/// </summary>
internal record Binding
{
    public Binding(
        ExchangeName exchangeName,
        RoutingKey routingKey,
        IDictionary<string, object>? arguments = null)
    {
        ExchangeName = exchangeName;
        RoutingKey = routingKey;
        Arguments = arguments ?? new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Название обменника
    /// </summary>
    public ExchangeName ExchangeName { get; }
    
    /// <summary>
    /// Ключ маршрутизации
    /// </summary>
    public RoutingKey RoutingKey { get; }
    
    /// <summary>
    /// Аргументы
    /// </summary>
    public IDictionary<string, object> Arguments { get; }
}

internal class BindingEqualityComparer : IEqualityComparer<Binding>
{
    public static readonly BindingEqualityComparer Instance = new();
    
    public bool Equals(Binding? x, Binding? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        
        return x.ExchangeName.Equals(y.ExchangeName) &&
               x.RoutingKey.Equals(y.RoutingKey) &&
               x.Arguments.SequenceEqual(y.Arguments);
    }

    public int GetHashCode(Binding obj) => HashCode.Combine(obj.ExchangeName, obj.RoutingKey, obj.Arguments);
}