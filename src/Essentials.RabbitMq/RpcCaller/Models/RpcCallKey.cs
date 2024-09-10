using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RpcCaller.Models;

public record RpcCallKey(
    ConnectionName ConnectionName,
    ExchangeName ExchangeName,
    RoutingKey RoutingKey,
    RoutingKey ReplyTo);