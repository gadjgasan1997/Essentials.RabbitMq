using NLog;
using MediatR;
using Essentials.RabbitMq.Publisher;
using Sample.Server.Samples.Sample1.Events;
using MessageContext = Essentials.RabbitMq.Subscriber.MessageContext;

namespace Sample.Server.Samples.Sample1.Handlers;

public class TestMediatrHandler : IRequestHandler<TestInEvent>
{
    private readonly IEventsPublisher _eventsPublisher;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    public TestMediatrHandler(IEventsPublisher eventsPublisher)
    {
        _eventsPublisher = eventsPublisher;
    }
    
    public async Task Handle(TestInEvent inEvent, CancellationToken cancellationToken)
    {
        var args = MessageContext.Current!.EventArgs;
        
        _logger.Info(
            $"{typeof(TestMediatrHandler).FullName} handled with " +
            $"correlationId '{args.BasicProperties?.CorrelationId}', reply to '{args.BasicProperties?.ReplyTo}' " +
            $"and message '{inEvent.Sample1Message}'");

        _ = await _eventsPublisher.PublishAsync(new TestOutEvent("response")).IfFailThrow();
    }
}