using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber.Exceptions;

/// <summary>
/// Исключение о не найденных опциях обработки события
/// </summary>
public class SubscriptionOptionsNotFound : KeyNotFoundException
{
    internal SubscriptionOptionsNotFound(SubscriptionKey key)
        : base($"Для подписки на событие с ключом '{key}' не найдены опции обработки")
    { }
}