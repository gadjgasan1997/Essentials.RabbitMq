using MediatR;
using Essentials.RabbitMq;

namespace Sample.Server.Samples.RpcCallSample.Events;

public class TestInEvent : IEvent, IRequest
{
    public string? Message { get; set; }
}