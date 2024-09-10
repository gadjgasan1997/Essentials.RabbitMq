using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.Subscriber.Models;

/// <summary>
/// Ключ подписки на событие
/// </summary>
/// <param name="ConnectionName">Название соединения</param>
/// <param name="QueueName">Название очереди</param>
public record SubscriptionKey(string ConnectionName, string QueueName);

/// <summary>
/// Ключ подписки на событие
/// </summary>
/// <param name="ConnectionName">Название соединения</param>
/// <param name="QueueName">Название очереди</param>
internal record InternalSubscriptionKey(ConnectionName ConnectionName, QueueName QueueName);