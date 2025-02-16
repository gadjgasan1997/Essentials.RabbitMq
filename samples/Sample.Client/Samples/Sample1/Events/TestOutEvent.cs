using Essentials.RabbitMq;

namespace Sample.Client.Samples.Sample1.Events;

public class TestOutEvent : IEvent
{
    public string? Message { get; set; }
}