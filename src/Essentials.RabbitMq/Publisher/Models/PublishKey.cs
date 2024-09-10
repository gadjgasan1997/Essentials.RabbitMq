using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.Publisher.Models;

/// <summary>
/// Ключ публикации
/// </summary>
/// <param name="ConnectionName">Название соединения</param>
/// <param name="ExchangeName">Название обменника</param>
/// <param name="RoutingKey">Ключ маршрутизации</param>
public record PublishKey(
    string ConnectionName,
    string ExchangeName,
    string? RoutingKey = null);

/// <summary>
/// Ключ публикации
/// </summary>
/// <param name="ConnectionName">Название соединения</param>
/// <param name="ExchangeName">Название обменника</param>
/// <param name="RoutingKey">Ключ маршрутизации</param>
internal record InternalPublishKey(
    ConnectionName ConnectionName,
    ExchangeName ExchangeName,
    RoutingKey RoutingKey);