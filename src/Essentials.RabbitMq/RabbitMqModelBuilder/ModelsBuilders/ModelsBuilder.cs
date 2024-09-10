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
    public IModelsBuilder DeclareQueue(string queueName, Action<IQueueBuilder>? configure = null)
    {
        var name = new QueueName(queueName);
        var builder = new QueueBuilder(name);
        
        configure?.Invoke(builder);
        
        var queue = builder.Build();
        QueuesStorage.RegisterQueue(_connectionName, queue);
        
        return this;
    }
    
    /// <inheritdoc cref="IModelsBuilder.DeclareExchange" />
    public IModelsBuilder DeclareExchange(
        string exchangeName,
        ExchangeType exchangeType,
        Action<IExchangeBuilder>? configure = null)
    {
        var name = new ExchangeName(exchangeName);
        var builder = new ExchangeBuilder(name, exchangeType);
        
        configure?.Invoke(builder);
        
        var exchange = builder.Build();
        ExchangesStorage.RegisterExchange(_connectionName, exchange);
        
        return this;
    }
}