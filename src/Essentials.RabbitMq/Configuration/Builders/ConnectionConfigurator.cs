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
    private readonly ModelsBuilder _modelsBuilder;
    
    public ConnectionConfigurator(ConnectionName connectionName)
    {
        _connectionName = connectionName;
        _modelsBuilder = new ModelsBuilder(connectionName);
    }
    
    /// <inheritdoc cref="IConnectionConfigurator.ConfigureModels" />
    public IConnectionConfigurator ConfigureModels(Action<IModelsBuilder> configure)
    {
        configure.CheckNotNull("Действие конфигурации моделей RabbitMq не может быть пустым");
        configure(_modelsBuilder);
        return this;
    }
    
    /// <inheritdoc cref="IConnectionConfigurator.ConfigurePublish{TEvent}" />
    public IConnectionConfigurator ConfigurePublish<TEvent>(
        ExchangeName exchangeName,
        RoutingKey routingKey,
        Action<IPublishConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        var key = new PublishKey(_connectionName, exchangeName, routingKey);
        
        var configurator = new PublishConfigurator<TEvent>();
        configure?.Invoke(configurator);
        var options = configurator.BuildPublishOptions();
        
        Set<TEvent>(key, options);
        return this;
    }
    
    /// <inheritdoc cref="IConnectionConfigurator.SubscribeOn{TEvent}" />
    public IConnectionConfigurator SubscribeOn<TEvent>(
        QueueName queueName,
        Action<IQueueBuilder>? configureQueue = null,
        Action<ISubscriptionConfigurator<TEvent>>? configureSubscription = null)
        where TEvent : IEvent
    {
        if (configureQueue is not null)
            _modelsBuilder.DeclareQueue(queueName, configureQueue);
        
        var key = SubscriptionKey.Create(_connectionName, queueName);
        
        var configurator = new SubscriptionConfigurator<TEvent>();
        configureSubscription?.Invoke(configurator);
        var options = configurator.BuildSubscriptionOptions();
        
        Set<TEvent>(key, options);
        return this;
    }
}