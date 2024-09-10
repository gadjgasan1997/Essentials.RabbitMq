using LanguageExt;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;

namespace Essentials.RabbitMq.Publisher;

/// <summary>
/// Интерфейс для публикации событий
/// </summary>
public interface IEventsPublisher
{
    /// <summary>
    /// Публикует событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="key">Ключ публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, PublishKey key)
        where TEvent : IEvent;
    
    /// <summary>
    /// Публикует событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="key">Ключ публикации</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(
        TEvent @event,
        PublishKey key,
        Action<IPublishConfigurator<TEvent>> configure)
        where TEvent : IEvent;
    
    /// <summary>
    /// Публикует событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent;
    
    /// <summary>
    /// Публикует событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, Action<IPublishConfigurator<TEvent>> configure)
        where TEvent : IEvent;
}