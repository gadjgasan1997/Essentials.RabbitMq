using RabbitMQ.Client;
using System.Collections.Concurrent;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.RabbitMqConnections;

namespace Essentials.RabbitMq.Publisher.Implementations;

/// <inheritdoc cref="IConnectionsService" />
internal class ConnectionsService : IConnectionsService, IDisposable
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private static readonly ConcurrentDictionary<ConnectionName, Lazy<IModel>> _channels = [];
    
    public ConnectionsService(IRabbitMqConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory.CheckNotNull();
    }
    
    /// <inheritdoc cref="IConnectionsService.GetOrCreateChannel" />
    public IModel GetOrCreateChannel(ConnectionName connectionName)
    {
        return _channels
            .GetOrAdd(
                connectionName,
                _ =>
                {
                    var connection = _connectionFactory.GetConnection(connectionName);
                    var channel = connection.CreateModel();
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