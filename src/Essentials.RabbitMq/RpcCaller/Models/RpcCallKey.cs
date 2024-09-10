using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RpcCaller.Models;

public record RpcCallKey(
    string ConnectionName,
    string ExchangeName,
    string RoutingKey,
    string ReplyTo);

internal record InternalRpcCallKey(ConnectionName ConnectionName, RoutingKey RoutingKey);