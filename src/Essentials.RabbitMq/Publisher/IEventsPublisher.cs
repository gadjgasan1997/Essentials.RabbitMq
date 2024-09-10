using LanguageExt;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;
using Essentials.RabbitMq.Configuration.Builders;

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
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, PublishKey key, CancellationToken? token = null)
        where TEvent : IEvent;
    
    /// <summary>
    /// Публикует событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="key">Ключ публикации</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(
        TEvent @event,
        PublishKey key,
        Action<IPublishConfigurator<TEvent>> configure,
        CancellationToken? token = null)
        where TEvent : IEvent;
    
    /// <summary>
    /// Публикует событие.
    /// Для вызова этого метода обменник и ключ маршрутизации должны быть заданы предварительно с помощью вызова
    /// <see cref="IConnectionConfigurator.ConfigurePublish{TEvent}" /> на конфигураторе соединения.
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, CancellationToken? token = null)
        where TEvent : IEvent;
    
    /// <summary>
    /// Публикует событие.
    /// Для вызова этого метода обменник и ключ маршрутизации должны быть заданы предварительно с помощью вызова
    /// <see cref="IConnectionConfigurator.ConfigurePublish{TEvent}" /> на конфигураторе соединения.
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    TryAsync<Unit> PublishAsync<TEvent>(
        TEvent @event,
        Action<IPublishConfigurator<TEvent>> configure,
        CancellationToken? token = null)
        where TEvent : IEvent;
}