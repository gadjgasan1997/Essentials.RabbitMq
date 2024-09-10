using LanguageExt;
using RabbitMQ.Client.Events;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber;

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
    void RegisterEventHandler<TEvent>(SubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent;

    /// <summary>
    /// Обрабатывает событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="eventArgs">Аргументы события</param>
    /// <returns></returns>
    TryOptionAsync<Unit> HandleEventAsync(SubscriptionKey key, BasicDeliverEventArgs eventArgs);
}