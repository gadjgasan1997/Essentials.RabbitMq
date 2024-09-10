using Essentials.RabbitMq.RpcCaller;

namespace Sample.Client.Samples.RpcCallSample.Events;

public class TestInEvent : IRpcCallResponse<TestOutEvent>
{
    public string? Message { get; set; }
}