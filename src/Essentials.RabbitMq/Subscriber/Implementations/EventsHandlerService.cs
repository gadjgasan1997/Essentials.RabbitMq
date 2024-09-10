using LanguageExt;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Interception.Extensions;
using Essentials.RabbitMq.Serialization;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <inheritdoc cref="IEventsHandlerService" />
internal class EventsHandlerService : IEventsHandlerService
{
    private readonly IMessageSerializer _messageSerializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<IMessageBehavior> _behaviors;
    private static readonly ConcurrentDictionary<SubscriptionKey, List<SubscriptionOptions>> _consumersOptions = [];
    
    public EventsHandlerService(
        IMessageSerializer messageSerializer,
        IServiceScopeFactory scopeFactory,
        IEnumerable<IMessageBehavior> behaviors)
    {
        _messageSerializer = messageSerializer.CheckNotNull();
        _scopeFactory = scopeFactory;
        _behaviors = behaviors;
    }
    
    /// <inheritdoc cref="IEventsHandlerService.RegisterEventHandler{TEvent}" />
    public void RegisterEventHandler<TEvent>(SubscriptionKey key, SubscriptionOptions options)
        where TEvent : IEvent
    {
        options.SetDefaultHandler(DefaultHandler);
        
        _ = SaveSubscriptionOptions(key, options).IfFailThrow();
        
        return;
        
        async Task DefaultHandler(IServiceProvider provider, IEvent content)
        {
            var @event = (TEvent) content;
            
            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
            await handler.HandleAsync(@event);
        }
    }
    
    private static Try<Unit> SaveSubscriptionOptions(SubscriptionKey key, SubscriptionOptions options)
    {
        return () =>
        {
            _consumersOptions.AddOrUpdate(
                key,
                addValueFactory: _ => [options],
                updateValueFactory: (_, optionsList) =>
                {
                    optionsList.Add(options);
                    return optionsList;
                });
            
            return Unit.Default;
        };
    }
    
    /// <inheritdoc cref="IEventsHandlerService.HandleEventAsync" />
    public TryOptionAsync<Unit> HandleEventAsync(SubscriptionKey key, BasicDeliverEventArgs eventArgs)
    {
        return GetSubscriptionOptions(key, eventArgs.RoutingKey, eventArgs.BasicProperties.Headers)
            .MapAsync(options => HandleEventAsync(options, eventArgs));
    }
    
    private static TryOption<SubscriptionOptions> GetSubscriptionOptions(
        SubscriptionKey subscriptionKey,
        string routingKey,
        IDictionary<string, object?>? headers)
    {
        return () =>
        {
            if (!_consumersOptions.TryGetValue(subscriptionKey, out var optionsList) || optionsList.Count == 0)
                throw new SubscriptionOptionsNotFound(subscriptionKey);
            
            if (optionsList.Count == 1)
                return optionsList[0];
            
            var hashSet = CreateHeadersSet(headers);
            return optionsList.FirstOrDefault(SearchOptions) ?? Option<SubscriptionOptions>.None;
            
            bool SearchOptions(SubscriptionOptions options) =>
                options.WhenPredicate?.Invoke(routingKey, hashSet) ?? false;
        };
    }
    
    private static System.Collections.Generic.HashSet<MessageHeader> CreateHeadersSet(
        IDictionary<string, object?>? headers)
    {
        if (headers is null || headers.Count == 0)
            return [];
        
        return headers.Select(MessageHeader.Create).ToHashSet();
    }
    
    private async Task<Unit> HandleEventAsync(SubscriptionOptions options, BasicDeliverEventArgs eventArgs)
    {
        var handler = options.Handler.CheckNotNull();
        
        await options.Behaviors
            .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
            .OfType<IMessageBehavior>()
            .InvokeAsync(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var @event = _messageSerializer.Deserialize(eventArgs.Body, options.EventType, options.ContentType);
                await handler(scope.ServiceProvider, @event);
            });
        
        return Unit.Default;
    }
}