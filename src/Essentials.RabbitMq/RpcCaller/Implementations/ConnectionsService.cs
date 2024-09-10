using RabbitMQ.Client;
using System.Collections.Concurrent;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.RabbitMqConnections;

namespace Essentials.RabbitMq.RpcCaller.Implementations;

/// <inheritdoc cref="IConnectionsService" />
internal class ConnectionsService : IConnectionsService, IDisposable
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private static readonly ConcurrentDictionary<ConnectionName, IModel> _channels = [];
    
    public ConnectionsService(IRabbitMqConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory.CheckNotNull();
    }
    
    /// <inheritdoc cref="IConnectionsService.GetOrCreateChannelForPublish" />
    public IModel GetOrCreateChannelForPublish(ConnectionName connectionName)
    {
        if (_channels.TryGetValue(connectionName, out var channel))
            return channel;
        
        var connection = _connectionFactory.GetConnection(connectionName);
        
        channel = connection.CreateModel();
        _channels[connectionName] = channel;
        
        return channel;
    }

    public IModel GetOrCreateChannelForConsume(RoutingKey replyTo)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        foreach (var model in _channels.Select(pair => pair.Value))
        {
            model.Close();
            model.Dispose();
        }
    }
}