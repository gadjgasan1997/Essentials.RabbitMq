using Essentials.RabbitMq.Models;
using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Publisher.Models;

/// <summary>
/// Ключ публикации
/// </summary>
public record PublishKey
{
    private PublishKey(
        ConnectionName connectionName,
        ExchangeName exchangeName,
        RoutingKey routingKey)
    {
        ConnectionName = connectionName;
        ExchangeName = exchangeName;
        RoutingKey = routingKey;
    }
    
    /// <summary>
    /// Название соединения
    /// </summary>
    public ConnectionName ConnectionName { get; }
    
    /// <summary>
    /// Название обменника
    /// </summary>
    public ExchangeName ExchangeName { get; }
    
    /// <summary>
    /// Ключ маршрутизации
    /// </summary>
    public RoutingKey RoutingKey { get; }
    
    /// <summary>
    /// Создает ключ публикации
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="exchangeName">Название обменника</param>
    /// <param name="routingKey">Ключ маршрутизации</param>
    public static PublishKey Create(
        ConnectionName connectionName,
        ExchangeName exchangeName,
        RoutingKey routingKey)
    {
        connectionName.CheckNotNull();
        exchangeName.CheckNotNull();
        routingKey.CheckNotNull();
        
        return new PublishKey(connectionName, exchangeName, routingKey);
    }
    
    /// <summary>
    /// Создает ключ публикации с пустым ключом маршрутизации
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="exchangeName">Название обменника</param>
    public static PublishKey Create(ConnectionName connectionName, ExchangeName exchangeName) =>
        Create(connectionName, exchangeName, RoutingKey.Empty);
    
    internal void Deconstruct(
        out ConnectionName connectionName,
        out ExchangeName exchangeName,
        out RoutingKey routingKey)
    {
        connectionName = ConnectionName;
        exchangeName = ExchangeName;
        routingKey = RoutingKey;
    }
}