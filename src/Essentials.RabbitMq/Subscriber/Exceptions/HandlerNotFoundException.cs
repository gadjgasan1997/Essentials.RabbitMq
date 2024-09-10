using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber.Exceptions;

/// <summary>
/// Исключение о не найденном обработчике события
/// </summary>
public class HandlerNotFoundException : Exception
{
    internal HandlerNotFoundException(InternalSubscriptionKey subscriptionKey)
        : base($"Для события с ключом '{subscriptionKey}' Не найден обработчик")
    { }
}