using NLog;
using Essentials.RabbitMq.Handlers;
using Sample.Server.Samples.Sample1.Events;

namespace Sample.Server.Samples.Sample1.Handlers;

public class TestEventHandler : IEventHandler<TestInEvent>
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    public async Task HandleAsync(TestInEvent inEvent)
    {
        _logger.Info($"{typeof(TestEventHandler).FullName} handled with message: '{inEvent.Sample1Message}'");
    }
}