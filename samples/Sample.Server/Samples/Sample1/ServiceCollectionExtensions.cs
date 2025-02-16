using System.Reflection;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Configuration.Builders;
using Sample.Server.Samples.Sample1.Events;
using Sample.Server.Samples.Sample1.Handlers;

namespace Sample.Server.Samples.Sample1;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSample1Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddTransient<TestEventHandler>()
            .AddTransient<RabbitMqEventHandler<TestInEvent>>();
        
        services
            .AddMediatR(serviceConfiguration =>
                serviceConfiguration.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()))
            .AddSingleton<Sample1Service>()
            .ConfigureRabbitMqConnections(configuration, Configure);
        
        // Вызывать после вызова метода конфигурации библиотеки!
        services.AddHostedService<Service>();
    }

    private static void Configure(IRabbitMqConfigurator configurator)
    {
        configurator.ConfigureConnection("esb", ConfigureEsbConnection);
    }

    private static void ConfigureEsbConnection(IConnectionConfigurator configurator)
    {
        configurator.ConfigurePublish<TestOutEvent>(
            exchangeName: KnownExchanges.AMQ_DIRECT,
            routingKey: RoutingKey.Empty,
            configure: publishConfigurator =>
                publishConfigurator
                    .AttachDefaultLoggingBehavior()
                    .WithJsonContentType()
                    .SetRetryCount(3));
    }
}