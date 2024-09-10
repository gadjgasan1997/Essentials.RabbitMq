using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Configuration.Builders;
using Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;
using static Essentials.RabbitMq.Publisher.Configuration.Storage;
using static Essentials.RabbitMq.Subscriber.Configuration.Storage;

namespace Essentials.RabbitMq.Configuration.Builders;

/// <inheritdoc cref="IConnectionConfigurator" />
internal class ConnectionConfigurator : IConnectionConfigurator
{
    private readonly ConnectionName _connectionName;
    
    public ConnectionConfigurator(ConnectionName connectionName)
    {
        _connectionName = connectionName;
    }
    
    /// <inheritdoc cref="IConnectionConfigurator.ConfigureModels" />
    public IConnectionConfigurator ConfigureModels(Action<IModelsBuilder> configure)
    {
        configure.CheckNotNull("Действие конфигурации моделей RabbitMq не может быть пустым");
        
        var modelsBuilder = new ModelsBuilder(_connectionName);
        configure(modelsBuilder);
        
        return this;
    }
    
    /// <inheritdoc cref="IConnectionConfigurator.ConfigurePublishEvent{TEvent}" />
    public IConnectionConfigurator ConfigurePublishEvent<TEvent>(
        string exchangeName,
        string? routingKey = null,
        Action<IPublishConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        var exchangeNameModel = new ExchangeName(exchangeName);
        var routingKeyModel = new RoutingKey(routingKey);
        
        var publishKey = new InternalPublishKey(
            _connectionName,
            exchangeNameModel,
            routingKeyModel);
        
        var configurator = new PublishConfigurator<TEvent>();
        configure?.Invoke(configurator);
        var publishOptions = configurator.BuildPublishOptions();
        
        Set<TEvent>(publishKey, publishOptions);
        
        return this;
    }
    
    /// <inheritdoc cref="IConnectionConfigurator.SubscribeOn{TEvent}" />
    public IConnectionConfigurator SubscribeOn<TEvent>(
        string queueName,
        Action<IQueueBuilder>? configureQueue = null,
        Action<ISubscriptionConfigurator<TEvent>>? configureSubscription = null)
        where TEvent : IEvent
    {
        var queueNameModel = new QueueName(queueName);
        var subscriptionKey = new InternalSubscriptionKey(_connectionName, queueNameModel);
        
        var modelsBuilder = new ModelsBuilder(_connectionName);
        modelsBuilder.DeclareQueue(queueName, configureQueue);
        
        var configurator = new SubscriptionConfigurator<TEvent>();
        configureSubscription?.Invoke(configurator);
        var subscriptionOptions = configurator.BuildSubscriptionOptions();
        
        Set<TEvent>(subscriptionKey, subscriptionOptions);
        
        return this;
    }
}