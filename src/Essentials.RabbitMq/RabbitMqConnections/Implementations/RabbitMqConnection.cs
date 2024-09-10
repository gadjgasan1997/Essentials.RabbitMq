using Polly;
using System.Reflection;
using System.Net.Sockets;
using System.Diagnostics.CodeAnalysis;
using Essentials.RabbitMq.Options;
using Essentials.Utils.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Essentials.RabbitMq.RabbitMqConnections.Implementations;

/// <inheritdoc cref="IRabbitMqConnection" />
internal class RabbitMqConnection : IRabbitMqConnection
{
    private readonly object _locker = new();
    
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private readonly ConnectionOptions _connectionOptions;
    private readonly ILogger _logger;

    public RabbitMqConnection(
        IConnectionFactory connectionFactory,
        ConnectionOptions connectionOptions,
        ILogger logger)
    {
        _connectionFactory = connectionFactory.CheckNotNull();
        _connectionOptions = connectionOptions.CheckNotNull();
        _logger = logger.CheckNotNull();
    }
    
    [MemberNotNullWhen(true, nameof(_connection))]
    private bool IsConnected => _connection is {IsOpen: true} && !IsDisposed;
    
    private bool IsDisposed { get; set; }
    
    public void Dispose()
    {
        _logger.LogWarning("Уничтожение подключения к хосту '{host}'", _connectionOptions.Host);
        
        if (IsDisposed)
            return;
        
        IsDisposed = true;
        
        if (_connection is null)
            return;
        
        try
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;
            _connection.CallbackException -= OnCallbackException;
            _connection.ConnectionBlocked -= OnConnectionBlocked;
            
            _connection.Close(
                0,
                $"Уничтожение подключения к хосту '{_connectionOptions.Host}'");
            
            _connection.Dispose();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Во время уничтожения подключения к хосту '{host}' произошло исключение",
                _connectionOptions.Host);
        }
    }

    public IModel CreateModel()
    {
        var connection = CreateConnection();
        return connection.CreateModel();
    }

    private IConnection CreateConnection()
    {
        if (_connection is not null && !IsDisposed)
            return _connection;
        
        if (!TryConnect())
            throw new InvalidOperationException($"Не удалось подключиться к хосту '{_connectionOptions.Host}'");
        
        return _connection;
    }
    
    [MemberNotNullWhen(true, nameof(_connection))]
    private bool TryConnect()
    {
        _logger.LogInformation(
            "Происходит попытка клиента RabbitMq подключиться к хосту '{host}' ...",
            _connectionOptions.Host);
        
        if (IsConnected)
            return true;
        
        lock (_locker)
        {
            Policy
                .Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(
                    _connectionOptions.ConnectRetryCount ?? 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.2, retryAttempt)),
                    onRetry: (exception, time) =>
                    {
                        Dispose();
                        
                        _logger.LogWarning(
                            exception,
                            message: "Клиент RabbitMq не смог подключиться к хосту '{host}'. " +
                                     "Затрачено времени, в секундах '{seconds}': ({errorMessage})",
                            _connectionOptions.Host,
                            $"{time.TotalSeconds:n1}", exception.Message);
                    })
                .Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection(Assembly.GetEntryAssembly()?.FullName);
                    IsDisposed = false;
                });
            
            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                
                _logger.LogInformation(
                    "Клиент RabbitMq успешно установил соединение с хостом '{host}'",
                    _connectionOptions.Host);
                
                return true;
            }
            
            _logger.LogCritical(
                "Клиент RabbitMq не смог установить соединение с хостом '{host}'",
                _connectionOptions.Host);
            
            return false;
        }
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning(
            "A RabbitMQ connection to host '{host}' is shutdown. Trying to re-connect...",
            _connectionOptions.Host);
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogWarning(
            "A RabbitMQ connection to host '{host}' throws an exception. Trying to re-connect...",
            _connectionOptions.Host);
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs reason)
    {
        _logger.LogWarning(
            "A RabbitMQ connection to host '{host}' is on shutdown. Trying to re-connect...",
            _connectionOptions.Host);
    }
}