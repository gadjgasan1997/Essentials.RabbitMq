using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

/// <inheritdoc cref="IModelsBuilder" />
internal class ModelsBuilder : IModelsBuilder
{
    private readonly ConnectionName _connectionName;
    
    public ModelsBuilder(ConnectionName connectionName)
    {
        _connectionName = connectionName;
    }
    
    /// <inheritdoc cref="IModelsBuilder.DeclareQueue" />
    public IModelsBuilder DeclareQueue(QueueName queueName, Action<IQueueBuilder>? configure = null)
    {
        var builder = new QueueBuilder(queueName);
        
        configure?.Invoke(builder);
        
        var queue = builder.Build();
        QueuesStorage.RegisterQueue(_connectionName, queue);
        
        return this;
    }
    
    /// <inheritdoc cref="IModelsBuilder.DeclareExchange" />
    public IModelsBuilder DeclareExchange(
        ExchangeName exchangeName,
        ExchangeType exchangeType,
        Action<IExchangeBuilder>? configure = null)
    {
        var builder = new ExchangeBuilder(exchangeName, exchangeType);
        
        configure?.Invoke(builder);
        
        var exchange = builder.Build();
        ExchangesStorage.RegisterExchange(_connectionName, exchange);
        
        return this;
    }
}