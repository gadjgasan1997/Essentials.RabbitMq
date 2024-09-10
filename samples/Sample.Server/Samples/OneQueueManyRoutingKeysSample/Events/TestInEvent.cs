using Essentials.RabbitMq;

namespace Sample.Server.Samples.OneQueueManyRoutingKeysSample.Events;

public class TestInEvent : IEvent
{
    public string? Message { get; set; }
}