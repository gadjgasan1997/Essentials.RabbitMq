using MediatR;
using Essentials.RabbitMq;
using Essentials.Utils.Extensions;

namespace Sample.Server.Samples.Sample2.Events;

public class TestEvent : IEvent, IRequest
{
    public TestEvent(string sample2Message)
    {
        Sample2Message = sample2Message.CheckNotNullOrEmpty();
    }

    public string Sample2Message { get; }
}