using NLog;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Publisher;
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

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