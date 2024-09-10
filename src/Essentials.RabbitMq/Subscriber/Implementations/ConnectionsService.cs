using LanguageExt;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.RabbitMqConnections;
using StringHashSet = System.Collections.Generic.HashSet<string>;

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <inheritdoc cref="IConnectionsService" />
internal class ConnectionsService : IConnectionsService, IDisposable
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    
    private static readonly ConcurrentDictionary<InternalSubscriptionKey, IModel> _channels = [];
    private static readonly ConcurrentDictionary<string, InternalSubscriptionKey> _consumersToKeysMap = [];
    
    public ConnectionsService(IRabbitMqConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory.CheckNotNull();
    }
    
    /// <inheritdoc cref="IConnectionsService.GetOrCreateChannel" />
    public IModel GetOrCreateChannel(InternalSubscriptionKey key, SubscriptionOptions options)
    {
        if (_channels.TryGetValue(key, out var channel))
            return channel;

        var connection = _connectionFactory.GetConnection(key.ConnectionName);

        channel = connection.CreateModel();
        channel.BasicQos(0, options.PrefetchCount, false);
        
        _channels[key] = channel;

        return channel;
    }
    
    /// <inheritdoc cref="IConnectionsService.CreateConsumer" />
    public AsyncEventingBasicConsumer CreateConsumer(IModel channel, InternalSubscriptionKey key)
    {
        var queueName = key.QueueName.Value;
        var connectionName = key.ConnectionName.Value;
        var consumerKey = $"{connectionName}_{queueName}";
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            consumerTag: consumerKey);
        
        _consumersToKeysMap.TryAdd(consumerKey, key);
        return consumer;
    }
    
    /// <inheritdoc cref="IConnectionsService.GetSubscriptionKey" />
    public Option<InternalSubscriptionKey> GetSubscriptionKey(string[] consumerTags)
    {
        if (consumerTags.FirstOrDefault() is not { } consumerTag)
            return Option<InternalSubscriptionKey>.None;
        
        var pair = _consumersToKeysMap.FirstOrDefault(pair => pair.Key == consumerTag);
        return pair.Value ?? Option<InternalSubscriptionKey>.None;
    }
    
    public void Dispose()
    {
        foreach (var model in _channels.Select(pair => pair.Value))
        {
            model.Close();
            model.Dispose();
        }
        
        _consumersToKeysMap.Clear();
    }
}