using NLog;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Publisher;

namespace Sample.Server.Samples.RpcCallSample;

public class RpcCallSampleService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    private readonly IEventsPublisher _eventsPublisher;
        
    public RpcCallSampleService(IEventsPublisher eventsPublisher)
    {
        _eventsPublisher = eventsPublisher.CheckNotNull();
    }
    
    public async Task RunAsync()
    {
        
    }
}