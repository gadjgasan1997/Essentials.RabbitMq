using System.Reflection;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Configuration.Builders;
using Sample.Server.Samples.RpcCallSample.Events;
using static Essentials.RabbitMq.Dictionaries.KnownExchanges;

namespace Sample.Server.Samples.RpcCallSample;

public static class ServiceCollectionExtensions
{
    public static void ConfigureRpcCallSample(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(serviceConfiguration =>
            serviceConfiguration.RegisterServicesFromAssembly(
                Assembly.GetExecutingAssembly()));
        
        services.AddTransient<RpcCallSampleService>();
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
        configurator
            .SubscribeOn<TestInEvent>(
                queueName: "test-rpc-request-queue",
                configureQueue: queueBuilder =>
                    queueBuilder
                        .Durable()
                        .BindToAmqDirectDefault(),
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .AttachDefaultLoggingBehavior()
                        .WithDefaultHandler())
            .ConfigurePublish<TestOutEvent>(
                exchangeName: AMQ_DIRECT,
                routingKey: RoutingKey.Empty,
                configure: publishConfigurator =>
                    publishConfigurator
                        .AttachDefaultLoggingBehavior());
    }
}