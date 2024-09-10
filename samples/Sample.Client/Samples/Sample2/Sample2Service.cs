using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Publisher;
using Sample.Client.Samples.Sample2.Events;
// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Local

namespace Sample.Client.Samples.Sample2;

public class Sample2Service
{
    private readonly IEventsPublisher _eventsPublisher;
    
    public Sample2Service(IEventsPublisher eventsPublisher)
    {
        _eventsPublisher = eventsPublisher.CheckNotNull();
    }

    public async Task RunAsync()
    {
        //await RunSample1();
        await RunSample2();
        //await RunSample3();
    }

    private async Task RunSample1()
    {
        var testEvent = new NotRegisteredEvent
        {
            Message = "hello"
        };

        var result = await _eventsPublisher.PublishAsync(testEvent).Try();
    }

    private async Task RunSample2()
    {
        var testEvent = new TestEvent
        {
            Message = "hello"
        };

        var result = await _eventsPublisher.PublishAsync(testEvent).Try();
    }

    private async Task RunSample3()
    {
        var testEvent = new TestEvent
        {
            Message = "hello"
        };

        var result = await _eventsPublisher
            .PublishAsync(testEvent, configurator => configurator.WithXmlContentType())
            .Try();
    }
}