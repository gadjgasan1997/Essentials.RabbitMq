using Polly;
using LanguageExt;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Interception.Extensions;
using Essentials.RabbitMq.Serialization;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;
using SubscriberMessageContext = Essentials.RabbitMq.Subscriber.MessageContext;
using static Essentials.RabbitMq.Publisher.Configuration.Storage;
using static System.Environment;

namespace Essentials.RabbitMq.Publisher.Implementations;

/// <inheritdoc cref="IEventsPublisher" />
internal class EventsPublisher : IEventsPublisher
{
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IConnectionsService _connectionsService;
    private readonly IEnumerable<IMessageBehavior> _behaviors;
    private readonly ILogger _logger;
    
    public EventsPublisher(
        IMessageSerializer messageSerializer,
        IMessageBuilder messageBuilder,
        IConnectionsService connectionsService,
        IEnumerable<IMessageBehavior> behaviors,
        ILoggerFactory loggerFactory)
    {
        _messageSerializer = messageSerializer.CheckNotNull();
        _messageBuilder = messageBuilder.CheckNotNull();
        _connectionsService = connectionsService.CheckNotNull();
        _behaviors = behaviors;
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.Publisher");
    }
    
    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, PublishKey)" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, PublishKey key)
        where TEvent : IEvent
    {
        return async () =>
        {
            var options = new PublishConfigurator<TEvent>().BuildPublishOptions();
            return await PublishCoreAsync(@event, key, options).Try();
        };
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
            
            return await PublishCoreAsync(@event, key, options).Try();
        };
    }

    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent)" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        return async () =>
        {
            var key = GetPublishKey<TEvent>();
            var options = GetPublishOptions<TEvent>();
            return await PublishCoreAsync(@event, key, options).Try();
        };
    }

    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, Action{IPublishConfigurator{TEvent}})" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, Action<IPublishConfigurator<TEvent>> configure)
        where TEvent : IEvent
    {
        return async () =>
        {
            var key = GetPublishKey<TEvent>();
            
            var configurator = new PublishConfigurator<TEvent>();
            configure.Invoke(configurator);
            var options = configurator.BuildPublishOptions();
            
            return await PublishCoreAsync(@event, key, options).Try();
        };
    }
    
    private TryAsync<Unit> PublishCoreAsync<TEvent>(
        TEvent @event,
        PublishKey key,
        PublishOptions options)
        where TEvent : IEvent
    {
        return async () =>
        {
            @event.CheckNotNull();
            
            var resolvedKey = key with
            {
                RoutingKey = ResolveRoutingKey(key.RoutingKey)
            };
            
            MessageContext.CreateContext(resolvedKey, @event);
            
            await options.Behaviors
                .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
                .OfType<IMessageBehavior>()
                .InvokeAsync(() =>
                {
                    PublishWithPolicy(@event, resolvedKey, options);
                    return Task.CompletedTask;
                });
            
            return Unit.Default;
        };
    }
    
    private static RoutingKey ResolveRoutingKey(RoutingKey routingKey)
    {
        // Если ключ маршрутизации задан явно - используется он.
        // Иначе происходит попытка получить ключ маршрутизации из контекста обрабатываемого события.
        // Такое обычно происходит, если сообщение публикуется как ответ на запрос
        return routingKey | RoutingKey.Create(SubscriberMessageContext.Current?.ReplyTo);
    }
    
    private void PublishWithPolicy<TEvent>(
        TEvent @event,
        PublishKey key,
        PublishOptions options)
        where TEvent : IEvent
    {
        var channel = _connectionsService.GetOrCreateChannel(key.ConnectionName);
        var properties = _messageBuilder.EnsureProperties(
            channel.CreateBasicProperties(),
            options,
            SubscriberMessageContext.Current?.CorrelationId);
        
        var body = _messageSerializer.Serialize(@event, options.ContentType);
        
        Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<Exception>()
            .WaitAndRetry(
                retryCount: options.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, time) =>
                {
                    _logger.LogError(
                        exception,
                        "Не удалось опубликовать сообщение по происшествии {seconds} секунд." +
                        "{newLine}Название обменника: '{exchange}'." +
                        "{newLine}Ключ маршрутизации: '{routingKey}'.",
                        time.TotalSeconds,
                        NewLine, key.ExchangeName.Value,
                        NewLine, key.RoutingKey.Value);
                })
            .Execute(() =>
            {
                channel.BasicPublish(
                    exchange: key.ExchangeName.Value,
                    routingKey: key.RoutingKey.Value,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
    }
}