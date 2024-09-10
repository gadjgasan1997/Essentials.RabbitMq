using NLog;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Subscriber;
using Essentials.RabbitMq.Subscriber.Models;
using Sample.Server.Samples.Sample1.Events;
using Sample.Server.Samples.Sample1.Handlers;
// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Local

namespace Sample.Server.Samples.Sample1;

public class Sample1Service
{
    private readonly IEventsSubscriber _eventsSubscriber;
    
    public Sample1Service(IEventsSubscriber eventsSubscriber)
    {
        _eventsSubscriber = eventsSubscriber.CheckNotNull();
    }
    
    public Task RunAsync()
    {
        // Не запускать эти примеры одновременно
        //RunSample1();
        //RunSample2();
        RunSample3();
        //RunSample4();
        
        return Task.CompletedTask;
    }

    private void RunSample1()
    {
        var result = _eventsSubscriber
            .Subscribe<TestInEvent>(SubscriptionKey.Create("esb", "esb-queue"))
            .IfFailThrow();
    }

    private void RunSample2()
    {
        var result = _eventsSubscriber
            .Subscribe<TestInEvent>(
                SubscriptionKey.Create("esb", "esb-queue"),
                configure: configurator =>
                    configurator
                        .WithJsonContentType()
                        .AttachDefaultMetricsBehavior()
                        .AttachDefaultLoggingBehavior()
                        .WithHandler<TestEventHandler>())
            .IfFailThrow();
    }

    private void RunSample3()
    {
        var result = _eventsSubscriber
            .Subscribe<TestInEvent>(
                SubscriptionKey.Create("esb", "esb-queue"),
                configure: configurator =>
                    configurator
                        .WithJsonContentType()
                        .AttachDefaultMetricsBehavior()
                        .AttachDefaultLoggingBehavior()
                        .WithHandler<RabbitMqEventHandler<TestInEvent>>())
            .IfFailThrow();
    }

    private void RunSample4()
    {
        var result = _eventsSubscriber
            .Subscribe<TestInEvent>(
                SubscriptionKey.Create("esb", "esb-queue"),
                configure: configurator =>
                    configurator
                        .WithJsonContentType()
                        .AttachDefaultMetricsBehavior()
                        .AttachDefaultLoggingBehavior()
                        .WithHandler(@event => LogManager.GetCurrentClassLogger().Info(@event.Sample1Message)))
            .IfFailThrow();
    }
}