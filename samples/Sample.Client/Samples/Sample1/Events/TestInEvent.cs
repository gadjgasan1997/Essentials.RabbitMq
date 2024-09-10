using Essentials.RabbitMq;

namespace Sample.Client.Samples.Sample1.Events;

public class TestInEvent : IEvent
{
    public string? Message { get; set; }
}