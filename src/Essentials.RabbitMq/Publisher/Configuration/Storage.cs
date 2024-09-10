using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher.Configuration;

/// <summary>
/// Хранилище для конфигурации публикации событий
/// </summary>
internal static class Storage
{
    private static readonly Dictionary<Type, Tuple<PublishKey, PublishOptions>> _map = [];
    
    /// <summary>
    /// Регистрирует ключ и опции публикации события
    /// </summary>
    /// <param name="key">Ключ публикации</param>
    /// <param name="options">Опции публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    public static void Set<TEvent>(PublishKey key, PublishOptions options)
        where TEvent : IEvent
    {
        _map[typeof(TEvent)] = new Tuple<PublishKey, PublishOptions>(key, options);
    }
    
    /// <summary>
    /// Возвращает ключ публикации события
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns>Ключ публикации</returns>
    public static PublishKey GetPublishKey<TEvent>()
        where TEvent : IEvent
    {
        if (!_map.TryGetValue(typeof(TEvent), out var tuple))
            throw new KeyNotFoundException($"Не найден ключ публикации события с типом '{typeof(TEvent).FullName}'");
        
        return tuple.Item1;
    }
    
    /// <summary>
    /// Возвращает опции публикации события
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns>Опции публикации</returns>
    public static PublishOptions GetPublishOptions<TEvent>()
        where TEvent : IEvent
    {
        if (!_map.TryGetValue(typeof(TEvent), out var tuple))
            throw new KeyNotFoundException($"Не найдены опции публикации события с типом '{typeof(TEvent).FullName}'");
        
        return tuple.Item2;
    }
}