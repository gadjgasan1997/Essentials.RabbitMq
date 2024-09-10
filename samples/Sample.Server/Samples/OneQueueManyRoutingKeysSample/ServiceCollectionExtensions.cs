using NLog;
using System.Reflection;
using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Configuration.Builders;
using Sample.Server.Samples.OneQueueManyRoutingKeysSample.Events;

namespace Sample.Server.Samples.OneQueueManyRoutingKeysSample;

public static class ServiceCollectionExtensions
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    public static void ConfigureOneQueueManyRoutingKeysSample(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(serviceConfiguration =>
            serviceConfiguration.RegisterServicesFromAssembly(
                Assembly.GetExecutingAssembly()));
        
        services.AddTransient<OneQueueManyRoutingKeysSampleService>();
        services.ConfigureRabbitMqConnections(configuration, Configure);
        
        // Вызывать после вызова метода конфигурации библиотеки!
        services.AddHostedService<Service>();
    }
    
    private static void Configure(IRabbitMqConfigurator configurator)
    {
        configurator.ConfigureConnection("esb", ConfigureEsbConnection);
    }

    private static void ConfigureEsbConnection(IConnectionConfigurator configurator)
    {
        const string queueName = "queue-for-many-routing-keys";

        configurator
            .SubscribeOn<TestInEvent>(
                queueName,
                configureQueue: queueBuilder => queueBuilder.Durable().BindToAmqDirectDefault(),
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .WithJsonContentType()
                        .AttachDefaultLoggingBehavior()
                        .When((key, headers) => key == "routing-key-zero" && headers.Count > 0)
                        .WithHandler(@event =>
                        {
                            _logger.Info($"Handling event with message1: {@event.Message}");
                        }))
            .SubscribeOn<TestInEvent>(
                queueName,
                configureQueue: queueBuilder => queueBuilder.Durable().BindToAmqDirectDefault(),
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .WithJsonContentType()
                        .AttachDefaultLoggingBehavior()
                        .WhenHeadersContains(("x-message-id", "test-message-id-value"))
                        .WithHandler(@event =>
                        {
                            _logger.Info($"Handling event with message2: {@event.Message}");
                        }))
            .SubscribeOn<TestInEvent>(
                queueName,
                configureQueue: queueBuilder => queueBuilder.Durable().BindToAmqDirect("routing-key-1"),
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .WithJsonContentType()
                        .AttachDefaultLoggingBehavior()
                        .AttachDefaultMetricsBehavior()
                        .WhenRoutingKeyIs("routing-key-1")
                        .WithHandler(@event =>
                        {
                            _logger.Info($"Handling event with message3: {@event.Message}");
                        }))
            .SubscribeOn<TestInEvent>(
                queueName,
                configureQueue: queueBuilder => queueBuilder.Durable().BindToAmqDirectDefault(),
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .WithJsonContentType()
                        .AttachDefaultMetricsBehavior()
                        .WhenHeadersAreEqual(
                        [
                            ("x-message-id", "x-message-id-value")
                        ])
                        .WithHandler(@event =>
                        {
                            _logger.Info($"Handling event with message4: {@event.Message}");
                        }))
            .SubscribeOn<TestInEvent>(
                queueName,
                configureQueue: queueBuilder => queueBuilder.Durable().BindToAmqDirect("routing-key-2"),
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .WithJsonContentType()
                        .AttachDefaultMetricsBehavior()
                        .WhenRoutingKeyIs("routing-key-2")
                        .WithHandler(@event =>
                        {
                            _logger.Info($"Handling event with message5: {@event.Message}");
                        }));
    }
}