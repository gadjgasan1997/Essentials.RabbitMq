using System.Diagnostics.CodeAnalysis;
using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher.Configuration;

/// <summary>
/// Хранилище для конфигурации публикации событий
/// </summary>
internal static class Storage
{
    private static readonly Dictionary<Type, Tuple<InternalPublishKey, PublishOptions>> _map = [];
    
    /// <summary>
    /// Регистрирует ключ и опции публикации события
    /// </summary>
    /// <param name="key">Ключ публикации</param>
    /// <param name="options">Опции публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    public static void Set<TEvent>(InternalPublishKey key, PublishOptions options)
        where TEvent : IEvent
    {
        _map[typeof(TEvent)] = new Tuple<InternalPublishKey, PublishOptions>(key, options);
    }
    
    /// <summary>
    /// Пытается вернуть ключ и опции публикации события
    /// </summary>
    /// <param name="key">Ключ публикации</param>
    /// <param name="options">Опции публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public static bool TryGetValue<TEvent>(
        [NotNullWhen(true)] out InternalPublishKey? key,
        [NotNullWhen(true)] out PublishOptions? options)
        where TEvent : IEvent
    {
        if (!_map.TryGetValue(typeof(TEvent), out var tuple))
        {
            key = null;
            options = null;
            return false;
        }
        
        key = tuple.Item1;
        options = tuple.Item2;
        return true;
    }
}