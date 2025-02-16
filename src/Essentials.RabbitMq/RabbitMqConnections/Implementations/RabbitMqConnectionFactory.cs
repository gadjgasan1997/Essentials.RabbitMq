using RabbitMQ.Client;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Options;
using Microsoft.Extensions.Logging;

namespace Essentials.RabbitMq.RabbitMqConnections.Implementations;

/// <inheritdoc cref="IRabbitMqConnectionFactory" />
internal class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly ILogger _logger;
    private static readonly Dictionary<ConnectionName, IRabbitMqConnection> _connections = new();
    
    public RabbitMqConnectionFactory(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CheckNotNull().CreateLogger("Essentials.RabbitMq.ConnectionFactory");
    }
    
    /// <summary>
    /// Добавляет соединение
    /// </summary>
    /// <param name="options">Опции соединения</param>
    public void AddConnection(ConnectionOptions options)
    {
        var connectionName = ConnectionName.Create(options.Name);
        if (_connections.ContainsKey(connectionName))
            return;
        
        var connectionFactory = new ConnectionFactory
        {
            HostName = options.Host,
            UserName = options.UserName,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            Port = options.Port,
            DispatchConsumersAsync = options.DispatchConsumersAsync
        };
        
        new CryptoService(_logger).ConfigureSsl(connectionFactory, options.Ssl);
        
        var connection = new RabbitMqConnection(connectionFactory, options, _logger);
        _connections.Add(connectionName, connection);
    }
    
    /// <inheritdoc cref="IRabbitMqConnectionFactory.GetConnection" />
    public IRabbitMqConnection GetConnection(ConnectionName connectionName)
    {
        if (!_connections.TryGetValue(connectionName, out var connection))
            throw new KeyNotFoundException($"Не найдено соединение RabbitMq с названием '{connectionName}'");

        return connection;
    }
    
    public void Dispose()
    {
        foreach (var (_, connection) in _connections)
            connection.Dispose();
    }
}