using Polly;
using LanguageExt;
using RabbitMQ.Client;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Interception;
using Essentials.RabbitMq.Publisher.Configuration;
using Essentials.RabbitMq.Publisher.Configuration.Builders;
using SubscriberMessageContext = Essentials.RabbitMq.Subscriber.MessageContext;
using static System.Environment;

namespace Essentials.RabbitMq.Publisher.Implementations;

/// <inheritdoc cref="IEventsPublisher" />
internal class EventsPublisher : IEventsPublisher
{
    private readonly IConnectionsService _connectionsService;
    private readonly IEnumerable<IMessagePublishBehavior> _behaviors;
    private readonly ILogger _logger;
    
    public EventsPublisher(
        IConnectionsService connectionsService,
        IEnumerable<IMessagePublishBehavior> behaviors,
        ILoggerFactory loggerFactory)
    {
        _connectionsService = connectionsService.CheckNotNull();
        _behaviors = behaviors;
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.Publisher");
    }
    
    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, PublishKey)" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, PublishKey key)
        where TEvent : IEvent
    {
        return PublishWithKeyAsync(@event, key);
    }
    
    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, PublishKey, Action{IPublishConfigurator{TEvent}})" />
    public TryAsync<Unit> PublishAsync<TEvent>(
        TEvent @event,
        PublishKey key,
        Action<IPublishConfigurator<TEvent>> configure)
        where TEvent : IEvent
    {
        return async () =>
        {
            var configurator = new PublishConfigurator<TEvent>();
            configure.Invoke(configurator);
            var options = configurator.BuildPublishOptions();
            
            return await PublishWithKeyAsync(@event, key, options).Try();
        };
    }

    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent)" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        return PublishWithoutKeyAsync(@event);
    }

    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, Action{IPublishConfigurator{TEvent}})" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, Action<IPublishConfigurator<TEvent>> configure)
        where TEvent : IEvent
    {
        return async () =>
        {
            var configurator = new PublishConfigurator<TEvent>();
            configure.Invoke(configurator);
            var options = configurator.BuildPublishOptions();
            
            return await PublishWithoutKeyAsync(@event, options).Try();
        };
    }
    
    private TryAsync<Unit> PublishWithKeyAsync<TEvent>(
        TEvent @event,
        PublishKey publishKey,
        PublishOptions? options = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            @event.CheckNotNull();
            
            var connectionNameModel = new ConnectionName(publishKey.ConnectionName);
            var exchangeNameModel = new ExchangeName(publishKey.ExchangeName);
            var routingKeyModel = ResolveRoutingKey(new RoutingKey(publishKey.RoutingKey));
            
            options ??= new PublishConfigurator<TEvent>().BuildPublishOptions();
            
            MessageContext.CreateContext(publishKey, @event);
            
            await PublishPrivate(
                @event,
                connectionNameModel,
                exchangeNameModel,
                routingKeyModel,
                options);
            
            return Unit.Default;
        };
    }
    
    private TryAsync<Unit> PublishWithoutKeyAsync<TEvent>(TEvent @event, PublishOptions? options = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            if (!Storage.TryGetValue<TEvent>(out var publishKey, out var publishOptions))
            {
                throw new KeyNotFoundException(
                    $"Не найдены опции публикации события с типом '{typeof(TEvent).FullName}'. " +
                    "Укажите их явно при конфигурации библиотеки или вызовете метод публикации, " +
                    "принимающий ключ публикации на вход");
            }
            
            var routingKey = ResolveRoutingKey(publishKey.RoutingKey);
            
            MessageContext.CreateContext(
                new PublishKey(
                    publishKey.ConnectionName.Value,
                    publishKey.ExchangeName.Value,
                    routingKey.Value),
                @event);
            
            await PublishPrivate(
                @event,
                publishKey.ConnectionName,
                publishKey.ExchangeName,
                routingKey,
                options ?? publishOptions);
            
            return Unit.Default;
        };
    }
    
    private static RoutingKey ResolveRoutingKey(RoutingKey routingKey)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        // Если ключ маршрутизации задан явно - используется он
        if (!routingKey.IsEmpty)
            return routingKey;
        
        // Иначе происходит попытка получить ключ маршрутизации из контекста обрабатываемого события.
        // Такое обычно происходит, если сообщение публикуется как ответ на запрос
        return new RoutingKey(SubscriberMessageContext.Current?.ReplyTo);
    }
    
    private async Task PublishPrivate<TEvent>(
        TEvent @event,
        ConnectionName connectionName,
        ExchangeName exchangeName,
        RoutingKey routingKey,
        PublishOptions publishOptions)
        where TEvent : IEvent
    {
        await publishOptions.Behaviors
            .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
            .OfType<IMessagePublishBehavior>()
            .Aggregate(
                (MessagePublishDelegate) SeedPublisher,
                (next, behavior) => async () => await behavior.Handle(next))
            .Invoke();
        
        return;
        
        Task SeedPublisher()
        {
            PublishWithPolicy(
                @event,
                connectionName,
                exchangeName,
                routingKey,
                publishOptions);
            
            return Task.CompletedTask;
        }
    }

    private void PublishWithPolicy<TEvent>(
        TEvent @event,
        ConnectionName connectionName,
        ExchangeName exchangeName,
        RoutingKey routingKey,
        PublishOptions publishOptions)
        where TEvent : IEvent
    {
        var channel = _connectionsService.GetOrCreateChannel(connectionName);
        var properties = CreateProperties(channel, publishOptions);
        var message = @event.CreateMessage(publishOptions.ContentType);
        
        Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<Exception>()
            .WaitAndRetry(
                retryCount: publishOptions.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, time) =>
                {
                    _logger.LogError(
                        exception,
                        "Не удалось опубликовать сообщение по происшествии {seconds} секунд." +
                        "{newLine}Название обменника: '{exchange}'." +
                        "{newLine}Ключ маршрутизации: '{publishKey.RoutingKey?.Key}'.",
                        time.TotalSeconds,
                        NewLine, exchangeName.Value,
                        NewLine, routingKey.Value);
                })
            .Execute(() =>
            {
                channel.BasicPublish(
                    exchange: exchangeName.Value,
                    routingKey: routingKey.Value,
                    mandatory: true,
                    basicProperties: properties,
                    body: message);
            });
    }

    private static IBasicProperties CreateProperties(IModel channel, PublishOptions publishOptions)
    {
        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = (byte) publishOptions.DeliveryMode;
        properties.Headers = publishOptions.Headers;
        
        // Проставление параметра CorrelationId из контекста обрабатываемого сообщения.
        // Обычно происходит, если сообщение публикуется как ответ на запрос
        var correlationId = SubscriberMessageContext.Current?.CorrelationId;
        if (!string.IsNullOrWhiteSpace(correlationId))
            properties.CorrelationId = correlationId;
        
        // Проставление параметра CorrelationId из опций публикации.
        // Обычно происходит, если публикация сообщения инициирована с целью получить на него ответ.
        // Т.е. параметр обычно проставляется тем, кто запросил ответ и ждет
        if (!string.IsNullOrWhiteSpace(publishOptions.CorrelationId))
            properties.CorrelationId = publishOptions.CorrelationId;
        
        // Проставление параметра ReplyTo из опций публикации.
        // Обычно происходит, если публикация сообщения инициирована с целью получить на него ответ.
        // Т.е. параметр обычно проставляется тем, кто запросил ответ и ждет
        if (!string.IsNullOrWhiteSpace(publishOptions.ReplyTo))
            properties.ReplyTo = publishOptions.ReplyTo;
        
        publishOptions.ConfigureProperties?.Invoke(properties);
        return properties;
    }
}