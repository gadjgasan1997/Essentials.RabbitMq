using System.Reflection;
using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Configuration.Builders;
using Sample.Server.Samples.Sample2.Events;

namespace Sample.Server.Samples.Sample2;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSample2Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddMediatR(serviceConfiguration =>
                serviceConfiguration.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()))
            .AddSingleton<Sample2Service>()
            .ConfigureRabbitMqConnections(configuration, Configure);
        
        // Вызывать после вызова метода конфигурации библиотеки!
        services.AddHostedService<Service>();
    }

    private static void Configure(IRabbitMqConfigurator builder)
    {
        builder.ConfigureConnection("esb", ConfigureEsbConnection);
    }

    private static void ConfigureEsbConnection(IConnectionConfigurator configurator)
    {
        configurator
            .SubscribeOn<TestEvent>(
                queueName: "esb-queue",
                configureSubscription: subscriptionConfigurator =>
                    subscriptionConfigurator
                        .WithJsonContentType()
                        .AttachDefaultMetricsBehavior()
                        .AttachDefaultLoggingBehavior()
                        
                        // Применится последний обработчик
                        .WithHandler(e => Console.WriteLine(e.Sample2Message))
                        .WithHandler((provider, e) =>
                        {
                            Thread.Sleep(1000);
                            
                            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger("test.logger");
                            logger.LogInformation("{message}", e.Sample2Message);
                        })
                        .WithHandler(async e =>
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine(e.Sample2Message);
                            await Task.CompletedTask;
                        })
                        .WithHandler(async (provider, e) =>
                        {
                            Thread.Sleep(1000);
                            
                            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger("test.logger");
                            logger.LogInformation("{message}", e.Sample2Message);
                            
                            await Task.CompletedTask;
                        })
                        .WithDefaultHandler());
    }
}