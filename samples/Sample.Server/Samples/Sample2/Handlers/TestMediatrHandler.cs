using NLog;
using MediatR;
using Sample.Server.Samples.Sample2.Events;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Sample.Server.Samples.Sample2.Handlers;

public class TestMediatrHandler : IRequestHandler<TestEvent>
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    public async Task Handle(TestEvent @event, CancellationToken cancellationToken)
    {
        _logger.Info($"{typeof(TestMediatrHandler).FullName} handled with message: '{@event.Sample2Message}'");
    }
}