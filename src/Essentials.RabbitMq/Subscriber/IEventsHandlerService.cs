using RabbitMQ.Client.Events;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber;

internal record HandlerInfo(Func<IServiceProvider, IEvent, Task> Handler);

/// <summary>
/// Сервис обработки событий
/// </summary>
internal interface IEventsHandlerService
{
    /// <summary>
    /// Регистрирует обработчик события
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    void RegisterEventHandler<TEvent>(InternalSubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent;
    
    /// <summary>
    /// Обрабатывает событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <param name="eventArgs">Аргументы события</param>
    /// <returns></returns>
    Task HandleEvent(
        InternalSubscriptionKey key,
        SubscriptionOptions options,
        BasicDeliverEventArgs eventArgs);
}