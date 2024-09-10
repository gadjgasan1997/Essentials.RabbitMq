using RabbitMQ.Client.Events;
using Essentials.Serialization;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Exceptions;
using Essentials.RabbitMq.Subscriber.Interception;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <inheritdoc cref="IEventsHandlerService" />
internal class EventsHandlerService : IEventsHandlerService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<IMessageHandlerBehavior> _behaviors;
    private readonly Dictionary<InternalSubscriptionKey, HandlerInfo> _handlers = [];
    
    public EventsHandlerService(
        IServiceScopeFactory scopeFactory,
        IEnumerable<IMessageHandlerBehavior> behaviors)
    {
        _scopeFactory = scopeFactory.CheckNotNull();
        _behaviors = behaviors;
    }
    
    /// <inheritdoc cref="IEventsHandlerService.RegisterEventHandler{TEvent}" />
    public void RegisterEventHandler<TEvent>(InternalSubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent
    {
        _handlers.Add(key, new HandlerInfo(options.Handler ?? DefaultHandler));
        return;
        
        async Task DefaultHandler(IServiceProvider provider, IEvent content)
        {
            var @event = (TEvent) content;
            
            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
            await handler.HandleAsync(@event);
        }
    }
    
    /// <inheritdoc cref="IEventsHandlerService.HandleEvent" />
    public async Task HandleEvent(
        InternalSubscriptionKey key,
        SubscriptionOptions options,
        BasicDeliverEventArgs eventArgs)
    {
        if (!_handlers.TryGetValue(key, out var handlerInfo))
            throw new HandlerNotFoundException(key);
        
        await options.Behaviors
            .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
            .OfType<IMessageHandlerBehavior>()
            .Aggregate(
                (MessageHandlerDelegate) SeedHandler,
                (next, behavior) => async () => await NextHandler(next, behavior))
            .Invoke();
        
        return;
        
        async Task SeedHandler()
        {
            using var scope = _scopeFactory.CreateScope();
            var @event = GetEvent(eventArgs.Body, options.EventType, options.ContentType);
            await handlerInfo.Handler(scope.ServiceProvider, @event);
        }
        
        async Task NextHandler(MessageHandlerDelegate next, IMessageHandlerBehavior behavior) =>
            await behavior.Handle(next);
    }

    private static IEvent GetEvent(ReadOnlyMemory<byte> content, Type eventType, ContentType contentType)
    {
        var deserializer = contentType switch
        {
            ContentType.Json => EssentialsDeserializersFactory.TryGet(KnownRabbitMqDeserializers.JSON),
            ContentType.Xml => EssentialsDeserializersFactory.TryGet(KnownRabbitMqDeserializers.XML),
            _ => throw new KeyNotFoundException($"Не найден десериалайзер для типа содержимого '{contentType}'")
        };
        
        var result = deserializer.Deserialize(eventType, content);
        result.CheckNotNull("Результат после десериализации сообщения равен null");
        
        if (result is not IEvent @event)
        {
            throw new InvalidOperationException(
                $"Результат после десериализации не соответствует интерфейсу '{typeof(IEvent).FullName}'");
        }
        
        return @event;
    }
}