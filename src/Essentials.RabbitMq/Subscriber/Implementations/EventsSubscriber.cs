using LanguageExt;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Configuration.Builders;

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <inheritdoc cref="IEventsSubscriber" />
internal class EventsSubscriber : IEventsSubscriber
{
    private readonly InternalEventsSubscriber _internalEventsSubscriber;
    
    public EventsSubscriber(InternalEventsSubscriber internalEventsSubscriber)
    {
        _internalEventsSubscriber = internalEventsSubscriber.CheckNotNull();
    }
    
    /// <inheritdoc cref="IEventsSubscriber.Subscribe{TEvent}(SubscriptionKey)" />
    public Try<Unit> Subscribe<TEvent>(SubscriptionKey key)
        where TEvent : IEvent
    {
        return _internalEventsSubscriber.Subscribe<TEvent>(key);
    }
    
    /// <inheritdoc cref="IEventsSubscriber.Subscribe{TEvent}(SubscriptionKey, Action{ISubscriptionConfigurator{TEvent}})" />
    public Try<Unit> Subscribe<TEvent>(SubscriptionKey key, Action<ISubscriptionConfigurator<TEvent>> configure)
        where TEvent : IEvent
    {
        return () =>
        {
            configure.CheckNotNull();
            return _internalEventsSubscriber.Subscribe(key, configure).Try();
        };
    }
}