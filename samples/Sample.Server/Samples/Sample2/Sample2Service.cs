using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber;
// ReSharper disable UnusedVariable
// ReSharper disable NotAccessedField.Local

namespace Sample.Server.Samples.Sample2;

public class Sample2Service
{
    private readonly IEventsSubscriber _eventsSubscriber;
    
    public Sample2Service(IEventsSubscriber eventsSubscriber)
    {
        _eventsSubscriber = eventsSubscriber.CheckNotNull();
    }
    
    public Task RunAsync()
    {
        return Task.CompletedTask;
    }
}