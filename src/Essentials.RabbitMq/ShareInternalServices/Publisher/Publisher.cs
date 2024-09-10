using Polly;
using System.Net.Sockets;
using Essentials.RabbitMq.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using static System.Environment;

namespace Essentials.RabbitMq.ShareInternalServices.Publisher;

/// <inheritdoc cref="IPublisher" />
internal class Publisher : IPublisher
{
    private readonly ILogger _logger;
    
    public Publisher(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.Publisher");
    }
    
    /// <inheritdoc cref="IPublisher.PublishAsync" />
    public async Task PublishAsync(
        IModel channel,
        ExchangeName exchangeName,
        RoutingKey routingKey,
        ReadOnlyMemory<byte> body,
        IBasicProperties properties,
        int retryCount,
        CancellationToken? token = null)
    {
        await Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, time) =>
                {
                    _logger.LogError(
                        exception,
                        "Не удалось опубликовать сообщение по происшествии {seconds} секунд." +
                        "{newLine}Название обменника: '{exchange}'." +
                        "{newLine}Ключ маршрутизации: '{routingKey}'.",
                        time.TotalSeconds,
                        NewLine, exchangeName.Value,
                        NewLine, routingKey.Value);
                    
                    token?.ThrowIfCancellationRequested();
                })
            .ExecuteAsync(() =>
            {
                channel.BasicPublish(
                    exchange: exchangeName.Value,
                    routingKey: routingKey.Value,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
                
                return Task.CompletedTask;
            });
    }
}