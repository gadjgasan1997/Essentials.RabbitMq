using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Interception;
using Essentials.RabbitMq.Subscriber.Interception.Behaviors;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Essentials.RabbitMq.Subscriber.Configuration.Builders;

/// <inheritdoc cref="ISubscriptionConfigurator{TEvent}" />
internal class SubscriptionConfigurator<TEvent> : ISubscriptionConfigurator<TEvent>
    where TEvent : IEvent
{
    private ContentType? _contentType;
    private ushort? _prefetchCount;
    private readonly List<Type> _behaviors = [];
    private Func<IServiceProvider, IEvent, Task>? _handler;
    private Type? _handlerToRegister;
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithJsonContentType" />
    public ISubscriptionConfigurator<TEvent> WithJsonContentType()
    {
        _contentType = ContentType.Json;
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithXmlContentType" />
    public ISubscriptionConfigurator<TEvent> WithXmlContentType()
    {
        _contentType = ContentType.Xml;
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.SetRetryCount" />
    public ISubscriptionConfigurator<TEvent> SetRetryCount(ushort prefetchCount)
    {
        _prefetchCount = prefetchCount;
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.AttachBehavior{TBehavior}" />
    public ISubscriptionConfigurator<TEvent> AttachBehavior<TBehavior>()
        where TBehavior : IMessageHandlerBehavior
    {
        _behaviors.Add(typeof(TBehavior));
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.AttachDefaultMetricsBehavior" />
    public ISubscriptionConfigurator<TEvent> AttachDefaultMetricsBehavior()
    {
        _behaviors.Add(typeof(MetricsMessageHandlerBehavior));
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.AttachDefaultLoggingBehavior" />
    public ISubscriptionConfigurator<TEvent> AttachDefaultLoggingBehavior()
    {
        _behaviors.Add(typeof(LoggingMessageHandlerBehavior));
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithHandler{THandler}" />
    public ISubscriptionConfigurator<TEvent> WithHandler<THandler>()
        where THandler : IEventHandler<TEvent>
    {
        _handlerToRegister = typeof(THandler);
        _handler = async (provider, @event) =>
        {
            var handler = provider.GetRequiredService<THandler>();
            await handler.HandleAsync((TEvent) @event);
        };
        
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithDefaultHandler" />
    public ISubscriptionConfigurator<TEvent> WithDefaultHandler() =>
        WithHandler<RabbitMqEventHandler<TEvent>>();
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithHandler(Action{TEvent})" />
    public ISubscriptionConfigurator<TEvent> WithHandler(Action<TEvent>? handler)
    {
        if (handler is null)
            return this;
        
        _handlerToRegister = null;
        _handler = (_, @event) =>
        {
            handler((TEvent) @event);
            return Task.CompletedTask;
        };
        
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithHandler(Action{IServiceProvider, TEvent})" />
    public ISubscriptionConfigurator<TEvent> WithHandler(Action<IServiceProvider, TEvent>? handler)
    {
        if (handler is null)
            return this;
        
        _handlerToRegister = null;
        _handler = (provider, @event) =>
        {
            handler(provider, (TEvent) @event);
            return Task.CompletedTask;
        };
        
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithHandler(Func{TEvent, Task})" />
    public ISubscriptionConfigurator<TEvent> WithHandler(Func<TEvent, Task>? handler)
    {
        if (handler is null)
            return this;
        
        _handlerToRegister = null;
        _handler = (_, @event) => handler((TEvent) @event);
        
        return this;
    }
    
    /// <inheritdoc cref="ISubscriptionConfigurator{TEvent}.WithHandler(Func{IServiceProvider, TEvent, Task})" />
    public ISubscriptionConfigurator<TEvent> WithHandler(Func<IServiceProvider, TEvent, Task>? handler)
    {
        if (handler is null)
            return this;
        
        _handlerToRegister = null;
        _handler = (provider, @event) => handler(provider, (TEvent) @event);
        
        return this;
    }
    
    /// <summary>
    /// Билдит опции подписки на событие
    /// </summary>
    /// <returns>Опции подписки на событие</returns>
    public SubscriptionOptions BuildSubscriptionOptions()
    {
        return new SubscriptionOptions(
            typeof(TEvent),
            _contentType ?? ContentType.Json,
            _prefetchCount ?? 5,
            _behaviors,
            _handler,
            _handlerToRegister);
    }
}