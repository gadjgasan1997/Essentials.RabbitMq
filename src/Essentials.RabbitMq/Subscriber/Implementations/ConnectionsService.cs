using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.RabbitMqConnections;

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <inheritdoc cref="IConnectionsService" />
internal class ConnectionsService : IConnectionsService, IDisposable
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    
    private static readonly ConcurrentDictionary<SubscriptionKey, Lazy<IModel>> _channels = [];
    private static readonly ConcurrentDictionary<SubscriptionKey, Lazy<AsyncEventingBasicConsumer>> _consumers = [];
    
    public ConnectionsService(IRabbitMqConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory.CheckNotNull();
    }
    
    /// <inheritdoc cref="IConnectionsService.Consume" />
    public void Consume(
        SubscriptionKey key,
        SubscriptionOptions options,
        AsyncEventHandler<BasicDeliverEventArgs> receivedHandler)
    {
        var channel = GetOrCreateChannel(key, options);
        
        _ = _consumers
            .GetOrAdd(
                key,
                _ =>
                {
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.Received += receivedHandler;
                    
                    channel.BasicConsume(
                        queue: key.QueueName.Value,
                        autoAck: false,
                        consumer: consumer,
                        consumerTag: key.Value);
                    
                    return new Lazy<AsyncEventingBasicConsumer>(() => consumer);
                })
            .Value;
    }
    
    private IModel GetOrCreateChannel(SubscriptionKey key, SubscriptionOptions options)
    {
        return _channels
            .GetOrAdd(
                key,
                _ =>
                {
                    var connection = _connectionFactory.GetConnection(key.ConnectionName);
                    var channel = connection.CreateModel();
                    channel.BasicQos(0, options.PrefetchCount, false);
                    
                    return new Lazy<IModel>(() => channel);
                })
            .Value;
    }
    
    public void Dispose()
    {
        foreach (var model in _channels.Select(pair => pair.Value))
        {
            model.Value.Close();
            model.Value.Dispose();
        }
    }
}