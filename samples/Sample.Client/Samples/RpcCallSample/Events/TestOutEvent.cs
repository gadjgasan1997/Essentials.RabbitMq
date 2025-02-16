using Essentials.RabbitMq.RpcCaller;

namespace Sample.Client.Samples.RpcCallSample.Events;

public class TestOutEvent : IRpcCallRequest
{
    public TestOutEvent(string? message)
    {
        Message = message;
    }

    public string? Message { get; }
}