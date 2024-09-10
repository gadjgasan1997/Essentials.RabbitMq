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