using LanguageExt;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber;

/// <summary>
/// Менеджер для работы с конфигурацией
/// </summary>
internal interface IConfigurationManager
{
    /// <summary>
    /// Сохраняет опции подписки на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    Try<Unit> SaveSubscriptionOptions<TEvent>(InternalSubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent;
    
    /// <summary>
    /// Возвращает опции подписки на событие по ключу
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <returns></returns>
    Try<SubscriptionOptions> GetSubscriptionOptionsByKey(InternalSubscriptionKey key);
}