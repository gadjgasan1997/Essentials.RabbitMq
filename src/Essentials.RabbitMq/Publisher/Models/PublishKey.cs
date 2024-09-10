using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.Publisher.Models;

/// <summary>
/// Ключ публикации
/// </summary>
/// <param name="ConnectionName">Название соединения</param>
/// <param name="ExchangeName">Название обменника</param>
/// <param name="RoutingKey">Ключ маршрутизации</param>
public record PublishKey(
    ConnectionName ConnectionName,
    ExchangeName ExchangeName,
    RoutingKey RoutingKey);