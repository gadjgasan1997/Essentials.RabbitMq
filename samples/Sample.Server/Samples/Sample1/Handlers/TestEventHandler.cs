using NLog;
using Essentials.RabbitMq.Handlers;
using Sample.Server.Samples.Sample1.Events;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Sample.Server.Samples.Sample1.Handlers;

public class TestEventHandler : IEventHandler<TestInEvent>
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    public async ValueTask HandleAsync(TestInEvent inEvent)
    {
        _logger.Info($"{typeof(TestEventHandler).FullName} handled with message: '{inEvent.Sample1Message}'");
    }
}