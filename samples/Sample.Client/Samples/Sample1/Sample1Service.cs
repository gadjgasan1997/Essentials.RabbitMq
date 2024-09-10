using NLog;
using Essentials.Utils.Extensions;
using Essentials.Serialization.Helpers;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Publisher;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Extensions;
using Essentials.RabbitMq.Subscriber;
using Essentials.RabbitMq.Subscriber.Models;
using Sample.Client.Samples.Sample1.Events;
using MessageContext = Essentials.RabbitMq.Subscriber.MessageContext;
// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Local

namespace Sample.Client.Samples.Sample1;

public class Sample1Service
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IEventsPublisher _eventsPublisher;
    private readonly IEventsSubscriber _eventsSubscriber;
    
    public Sample1Service(
        IEventsPublisher eventsPublisher,
        IEventsSubscriber eventsSubscriber)
    {
        _eventsPublisher = eventsPublisher.CheckNotNull();
        _eventsSubscriber = eventsSubscriber.CheckNotNull();
    }
    
    public async Task RunAsync()
    {
        await RunSample1();
        //await RunSample2();
        //await RunSample3();
        //await RunSample4();
    }

    private async Task RunSample1()
    {
        var testEvent = new TestOutEvent
        {
            Message = "hello"
        };
        
        _ = await _eventsSubscriber
            .SubscribeAsync<TestOutEvent>(
                SubscriptionKey.Create("esb", "test-queue-out-dev1"),
                configure: configurator =>
                    configurator
                        .AttachDefaultLoggingBehavior()
                        .WithHandler(@event =>
                        {
                            var properties = MessageContext.Current!.EventArgs.BasicProperties;
                            
                            _logger.Info($"{@event.Message} ::: {properties.CorrelationId}");
                        }))
            .Try();
        
        var result1 = await _eventsPublisher
            .PublishAsync(
                testEvent,
                new PublishKey("esb", "amq.direct", "test-queue-out-dev1"),
                configure: configurator =>
                    configurator
                        .AttachHeader("test_header", "value")
                        .SetDeliveryMode(DeliveryMode.Persistent)
                        .SetCorrelationId(Guid.NewGuid().ToString())
                        .SetReplyTo("test-queue-out-dev1")
                        .WithJsonContentType()
                        .AttachDefaultLoggingBehavior()
                        .SetRetryCount(2))
            .Try();
    }

    private async Task RunSample2()
    {
        var testEvent = new TestOutEvent
        {
            Message = "hello to direct"
        };

        var result = await _eventsPublisher
            .PublishToAmqDirectAsync(
                connectionName: "esb",
                routingKey: "esb-queue-direct",
                @event: testEvent,
                configure: configurator =>
                    configurator
                        .SetDeliveryMode(DeliveryMode.Persistent)
                        .SetCorrelationId(Guid.NewGuid().ToString())
                        .WithJsonContentType()
                        .SetRetryCount(2))
            .Try();
    }

    private async Task RunSample3()
    {
        var testEvent = new TestOutEvent
        {
            Message = "hello to fanout"
        };

        var result = await _eventsPublisher
            .PublishToAmqFanoutAsync(
                connectionName: "esb",
                @event: testEvent,
                configure: configurator =>
                    configurator
                        .AttachHeader("test_header", "value")
                        .SetDeliveryMode(DeliveryMode.Persistent)
                        .WithJsonContentType()
                        .SetRetryCount(2))
            .Try();
    }

    private async Task RunSample4()
    {
        var testEvent = new TestOutEvent
        {
            Message = "hello to topic"
        };

        var result = await _eventsPublisher
            .PublishToAmqTopicAsync(
                connectionName: "esb",
                routingKey: "esb-queue-topic",
                @event: testEvent,
                configure: configurator =>
                    configurator
                        .AttachHeader("test_header", JsonHelpers.Serialize(testEvent))
                        .ConfigureProperties(properties => properties.CorrelationId = Guid.NewGuid().ToString())
                        .SetDeliveryMode(DeliveryMode.Persistent)
                        .WithXmlContentType())
            .Try();
    }
}