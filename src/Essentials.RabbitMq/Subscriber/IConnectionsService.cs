using RabbitMQ.Client.Events;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber;

/// <summary>
/// Сервис для работы с соединениями
/// </summary>
internal interface IConnectionsService
{
    /// <summary>
    /// Начинает слушать событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <param name="receivedHandler">Обработчик получения сообщения</param>
    void Consume(
        SubscriptionKey key,
        SubscriptionOptions options,
        AsyncEventHandler<BasicDeliverEventArgs> receivedHandler);
}