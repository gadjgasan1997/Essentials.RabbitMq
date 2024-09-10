using Essentials.RabbitMq;

namespace Sample.Client.Samples.Sample2.Events;

public class TestEvent : IEvent
{
    public string? Message { get; set; }
}