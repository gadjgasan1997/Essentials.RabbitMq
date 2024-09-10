using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using Essentials.Utils.Extensions;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static System.Environment;

namespace Essentials.RabbitMq.RpcCaller.Implementations;

internal class RpcCallResponseSubscriber
{
    private readonly IRpcTasksService _rpcTasksService;
    private readonly ILogger _logger;
    
    public RpcCallResponseSubscriber(
        IRpcTasksService rpcTasksService,
        ILoggerFactory loggerFactory)
    {
        _rpcTasksService = rpcTasksService.CheckNotNull();
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
            
            return;
        }
        
        //_rpcTasksService.SetResult()
    }
}