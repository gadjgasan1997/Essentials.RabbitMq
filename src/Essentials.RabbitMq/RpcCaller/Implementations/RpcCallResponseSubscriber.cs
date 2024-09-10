using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static System.Environment;

namespace Essentials.RabbitMq.RpcCaller.Implementations;

internal class RpcCallResponseSubscriber
{
    private readonly ILogger _logger;
    
    public RpcCallResponseSubscriber(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.RpcCallResponseSubscriber");
    }
    
    private async Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (sender is not AsyncEventingBasicConsumer consumer)
        {
            _logger.LogError(
                "Слушатель не является типом '{typeName}'" +
                "{newLine}Аргументы: '{args}'",
                typeof(AsyncEventingBasicConsumer).FullName,
                NewLine, Serialize(eventArgs));
        }
    }
}