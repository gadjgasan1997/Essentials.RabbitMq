using Essentials.RabbitMq;

namespace Sample.Client.Samples.Sample2.Events;

public class NotRegisteredEvent : IEvent
{
    public string? Message { get; set; }
}