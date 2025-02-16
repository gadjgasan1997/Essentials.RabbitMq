using LanguageExt;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Configuration.Builders;

namespace Essentials.RabbitMq.Subscriber;

/// <summary>
/// Интерфейс для подписки на события
/// </summary>
public interface IEventsSubscriber
{
    /// <summary>
    /// Подписывается на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> SubscribeAsync<TEvent>(SubscriptionKey key)
        where TEvent : IEvent;
    
    /// <summary>
    /// Подписывается на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="configure">Действие конфигурации опций подписки</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> SubscribeAsync<TEvent>(SubscriptionKey key, Action<ISubscriptionConfigurator<TEvent>> configure)
        where TEvent : IEvent;
}