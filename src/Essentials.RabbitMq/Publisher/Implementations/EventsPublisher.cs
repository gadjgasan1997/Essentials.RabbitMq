using LanguageExt;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Interception.Extensions;
using Essentials.RabbitMq.ShareInternalServices.Serializer;
using Essentials.RabbitMq.ShareInternalServices.Publisher;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;
using SubscriberMessageContext = Essentials.RabbitMq.Subscriber.MessageContext;
using static Essentials.RabbitMq.Publisher.Configuration.Storage;

namespace Essentials.RabbitMq.Publisher.Implementations;

/// <inheritdoc cref="IEventsPublisher" />
internal class EventsPublisher : IEventsPublisher
{
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IConnectionsService _connectionsService;
    private readonly IEnumerable<IMessageBehavior> _behaviors;
    private readonly IPublisher _publisher;
    
    public EventsPublisher(
        IMessageSerializer messageSerializer,
        IMessageBuilder messageBuilder,
        IConnectionsService connectionsService,
        IEnumerable<IMessageBehavior> behaviors,
        IPublisher publisher)
    {
        _messageSerializer = messageSerializer.CheckNotNull();
        _messageBuilder = messageBuilder.CheckNotNull();
        _connectionsService = connectionsService.CheckNotNull();
        _behaviors = behaviors;
        _publisher = publisher.CheckNotNull();
    }
    
    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, PublishKey, CancellationToken?)" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, PublishKey key, CancellationToken? token = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            var options = PublishConfigurator<TEvent>.DefaultOptions;
            return await PublishCoreAsync(@event, key, options, token).Try();
        };
    }
    
    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, PublishKey, Action{IPublishConfigurator{TEvent}}, CancellationToken?)" />
    public TryAsync<Unit> PublishAsync<TEvent>(
        TEvent @event,
        PublishKey key,
        Action<IPublishConfigurator<TEvent>> configure,
        CancellationToken? token = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            var options = new PublishConfigurator<TEvent>().BuildOptions(configure);
            return await PublishCoreAsync(@event, key, options, token).Try();
        };
    }

    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, CancellationToken?)" />
    public TryAsync<Unit> PublishAsync<TEvent>(TEvent @event, CancellationToken? token = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            var key = GetPublishKey<TEvent>();
            var options = GetPublishOptions<TEvent>();
            return await PublishCoreAsync(@event, key, options, token).Try();
        };
    }

    /// <inheritdoc cref="IEventsPublisher.PublishAsync{TEvent}(TEvent, Action{IPublishConfigurator{TEvent}}, CancellationToken?)" />
    public TryAsync<Unit> PublishAsync<TEvent>(
        TEvent @event,
        Action<IPublishConfigurator<TEvent>> configure,
        CancellationToken? token = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            var key = GetPublishKey<TEvent>();
            var options = new PublishConfigurator<TEvent>().BuildOptions(configure);
            return await PublishCoreAsync(@event, key, options, token).Try();
        };
    }
    
    private TryAsync<Unit> PublishCoreAsync<TEvent>(
        TEvent @event,
        PublishKey key,
        PublishOptions options,
        CancellationToken? token = null)
        where TEvent : IEvent
    {
        return async () =>
        {
            @event.CheckNotNull();
            
            var resolvedKey = PublishKey.Create(
                key.ConnectionName,
                key.ExchangeName,
                ResolveRoutingKey(key.RoutingKey));
            
            var channel = _connectionsService.GetOrCreateChannel(key.ConnectionName);
            var properties = _messageBuilder.EnsureProperties(
                channel.CreateBasicProperties(),
                options,
                SubscriberMessageContext.Current?.CorrelationId);
            
            var body = _messageSerializer.Serialize(@event, options.ContentType);
            
            MessageContext.CreateContext(resolvedKey, @event);
            
            await options.Behaviors
                .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
                .OfType<IMessageBehavior>()
                .InvokeAsync(async () => await SeedAsync());
            
            return Unit.Default;
            
            async Task SeedAsync()
            {
                await _publisher.PublishAsync(
                    channel,
                    key.ExchangeName,
                    key.RoutingKey,
                    body,
                    properties,
                    options.RetryCount,
                    token);
            }
        };
    }
    
    private static RoutingKey ResolveRoutingKey(RoutingKey routingKey)
    {
        // Если ключ маршрутизации задан явно - используется он.
        // Иначе происходит попытка получить ключ маршрутизации из контекста обрабатываемого события.
        // Такое обычно происходит, если сообщение публикуется как ответ на запрос
        return routingKey | RoutingKey.Create(SubscriberMessageContext.Current?.ReplyTo);
    }
}