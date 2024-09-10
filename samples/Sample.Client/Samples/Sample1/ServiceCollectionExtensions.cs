using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Configuration.Builders;
using Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;
using static Essentials.RabbitMq.Dictionaries.KnownExchanges;

namespace Sample.Client.Samples.Sample1;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSample1Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<Sample1Service>();
        services.ConfigureRabbitMqConnections(configuration, Configure);
        
        // Вызывать после вызова метода конфигурации библиотеки!
        services.AddHostedService<Service>();
    }

    private static void Configure(IRabbitMqConfigurator configurator)
    {
        configurator.ConfigureConnection(
            "esb",
            configureConnection: builder => builder.ConfigureModels(ConfigureModels));
    }

    private static void ConfigureModels(IModelsBuilder builder)
    {
        builder
            .DeclareQueue(
                "esb-queue",
                queueBuilder => queueBuilder.Bind(AMQ_DIRECT, "esb-queue"))
            .DeclareQueue(
                "test-queue-out-dev1",
                queueBuilder => queueBuilder.Bind(AMQ_DIRECT, "test-queue-out-dev1"));
        
        builder.DeclareQueue(
            "esb-queue-direct",
            queueBuilder => queueBuilder.BindToAmqDirect("esb-queue-direct"));
        
        builder
            .DeclareQueue(
                "esb-queue-fanout-1",
                queueBuilder => queueBuilder.BindToFanoutDirect())
            .DeclareQueue(
                "esb-queue-fanout-2",
                queueBuilder => queueBuilder.BindToFanoutDirect());
        
        builder.DeclareQueue(
            "esb-queue-topic",
            queueBuilder => queueBuilder.BindToAmqTopic("esb-queue-topic"));
    }
}