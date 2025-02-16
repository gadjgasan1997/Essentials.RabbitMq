using Essentials.RabbitMq;

namespace Sample.Server.Samples.RpcCallSample.Events;

public class TestOutEvent : IEvent
{
    public TestOutEvent(string message)
    {
        Message = message;
    }
    
    public string Message { get; }
}