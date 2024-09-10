using MediatR;
using Essentials.RabbitMq;
using Essentials.Utils.Extensions;

namespace Sample.Server.Samples.Sample1.Events;

public class TestInEvent : IEvent, IRequest
{
    public TestInEvent(string sample1Message)
    {
        Sample1Message = sample1Message.CheckNotNullOrEmpty();
    }

    public string Sample1Message { get; }
}