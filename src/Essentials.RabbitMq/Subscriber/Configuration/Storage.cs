using System.Collections;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber.Configuration;

/// <summary>
/// Хранилище для конфигурации подписок на события
/// </summary>
internal class Storage : IEnumerable<(Type, SubscriptionKey, SubscriptionOptions)>
{
    private static readonly Dictionary<Type, Tuple<SubscriptionKey, List<SubscriptionOptions>>> _map = [];
    
    /// <summary>
    /// Регистрирует ключ и опции подписки на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    public static void Set<TEvent>(SubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent
    {
        if (_map.TryGetValue(typeof(TEvent), out var tuple))
        {
            tuple.Item2.Add(options);
            return;
        }
        
        _map[typeof(TEvent)] = new Tuple<SubscriptionKey, List<SubscriptionOptions>>(key, [options]);
    }

    public IEnumerator<(Type, SubscriptionKey, SubscriptionOptions)> GetEnumerator() =>
        _map
            .SelectMany(pair => pair.Value.Item2.Select(options => (pair.Key, pair.Value.Item1, options)))
            .GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}