﻿using LanguageExt;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;

namespace Essentials.RabbitMq.Publisher.Extensions;

/// <summary>
/// Методы расширения для <see cref="IEventsPublisher" />
/// </summary>
public static class EventsPublisherExtensions
{
    /// <summary>
    /// Публикует событие в обменник <see cref="Exchange.AmqDirect" />
    /// </summary>
    /// <param name="publisher">Интерфейс для публикации событий</param>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="event">Событие</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public static TryAsync<Unit> PublishToAmqDirectAsync<TEvent>(
        this IEventsPublisher publisher,
        string connectionName,
        string routingKey,
        TEvent @event,
        Action<IPublishConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        var publishKey = new PublishKey(connectionName, Exchange.AmqDirect.Name.Value, routingKey);
        return publisher.PublishAsync(@event, publishKey, configure);
    }
    
    /// <summary>
    /// Публикует событие в обменник <see cref="Exchange.AmqFanout" />
    /// </summary>
    /// <param name="publisher">Интерфейс для публикации событий</param>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="event">Событие</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public static TryAsync<Unit> PublishToAmqFanoutAsync<TEvent>(
        this IEventsPublisher publisher,
        string connectionName,
        TEvent @event,
        Action<IPublishConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        var publishKey = new PublishKey(connectionName, Exchange.AmqFanout.Name.Value);
        return publisher.PublishAsync(@event, publishKey, configure);
    }
    
    /// <summary>
    /// Публикует событие в обменник <see cref="Exchange.AmqTopic" />
    /// </summary>
    /// <param name="publisher">Интерфейс для публикации событий</param>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="event">Событие</param>
    /// <param name="configure">Действие конфигурации опций публикации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public static TryAsync<Unit> PublishToAmqTopicAsync<TEvent>(
        this IEventsPublisher publisher,
        string connectionName,
        string routingKey,
        TEvent @event,
        Action<IPublishConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        var publishKey = new PublishKey(connectionName, Exchange.AmqTopic.Name.Value, routingKey);
        return publisher.PublishAsync(@event, publishKey, configure);
    }
}