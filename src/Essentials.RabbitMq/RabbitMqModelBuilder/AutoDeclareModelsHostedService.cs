using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Essentials.Utils.Extensions;
using Essentials.Logging.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.RabbitMqConnections;

namespace Essentials.RabbitMq.RabbitMqModelBuilder;

/// <summary>
/// Сервис для автоматического объявления моделей RabbitMq
/// </summary>
internal class AutoDeclareModelsHostedService : IHostedService
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly ILogger _logger;
    
    public AutoDeclareModelsHostedService(
        IRabbitMqConnectionFactory connectionFactory,
        ILoggerFactory loggerFactory)
    {
        _connectionFactory = connectionFactory.CheckNotNull();
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.ModelsBuilder");
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var (connectionName, queues) in new QueuesStorage())
            {
                var connection = _connectionFactory.GetConnection(connectionName);
                foreach (var queue in queues)
                    RegisterQueue(connection, connectionName, queue);
            }
            
            foreach (var (connectionName, exchanges) in new ExchangesStorage())
            {
                var connection = _connectionFactory.GetConnection(connectionName);
                foreach (var exchange in exchanges)
                    RegisterExchange(connection, connectionName, exchange);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Во время автоматического объявления моделей RabbitMq произошло исключение");
            
            throw;
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private void RegisterQueue(
        IRabbitMqConnection connection,
        ConnectionName connectionName,
        Queue queue)
    {
        try
        {
            _logger.LogIfLevelIsInfoOrLow(() =>
            {
                _logger.LogInformation(
                    "Происходит объявление очереди '{queueName}' в соединении '{connectionName}'",
                    queue.Name.Value, connectionName.Value);
            });
            
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue.Name.Value,
                queue.Durable,
                queue.Exclusive,
                queue.AutoDelete,
                queue.Arguments);
            
            AddQueueBindings(channel, queue);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Во время автоматического объявления очереди с названием '{queueName}' " +
                "в соединении '{connectionName}' произошло исключение",
                queue.Name.Value, connectionName.Value);
            
            throw;
        }
    }
    
    private void AddQueueBindings(IModel channel, Queue queue)
    {
        foreach (var binding in queue.Bindings)
        {
            _logger.LogIfLevelIsInfoOrLow(() =>
            {
                _logger.LogInformation(
                    "Происходит привязки очереди '{queueName}' к обменнику '{exchangeName}' с ключом маршрутизации '{routingKey}'",
                    queue.Name.Value, binding.ExchangeName.Value, binding.RoutingKey.Value);
            });
            
            channel.QueueBind(
                queue.Name.Value,
                binding.ExchangeName.Value,
                binding.RoutingKey.Value,
                binding.Arguments);
        }
    }
    
    private void RegisterExchange(
        IRabbitMqConnection connection,
        ConnectionName connectionName,
        Exchange exchange)
    {
        try
        {
            _logger.LogIfLevelIsInfoOrLow(() =>
            {
                _logger.LogInformation(
                    "Происходит объявление обменника '{queueName}' в соединении '{connectionName}'",
                    exchange.Name.Value, connectionName.Value);
            });
            
            using var channel = connection.CreateModel();
            
            channel.ExchangeDeclare(
                exchange.Name.Value,
                exchange.Type.DisplayName,
                exchange.Durable,
                exchange.AutoDelete,
                exchange.Arguments);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Во время автоматического объявления обменника с названием '{exchangeName}' " +
                "в соединении '{connectionName}' произошло исключение",
                exchange.Name.Value, connectionName.Value);
            
            throw;
        }
    }
}