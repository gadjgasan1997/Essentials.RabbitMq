using System.Collections;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber.Configuration;

/// <summary>
/// Хранилище для конфигурации подписок на события
/// </summary>
internal class Storage : IEnumerable<(Type, InternalSubscriptionKey, SubscriptionOptions)>
{
    private static readonly Dictionary<Type, Tuple<InternalSubscriptionKey, SubscriptionOptions>> _map = [];
    
    /// <summary>
    /// Регистрирует ключ и опции подписки на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    public static void Set<TEvent>(InternalSubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent
    {
        _map[typeof(TEvent)] = new Tuple<InternalSubscriptionKey, SubscriptionOptions>(key, options);
    }
    
    public IEnumerator<(Type, InternalSubscriptionKey, SubscriptionOptions)> GetEnumerator() =>
        _map
            .Select(pair => (pair.Key, pair.Value.Item1, pair.Value.Item2))
            .GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}