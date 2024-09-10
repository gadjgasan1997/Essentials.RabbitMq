using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Configuration.Builders;
using Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

namespace Sample.Client.Samples.Sample2;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSample2Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<Sample2Service>();
        services.ConfigureRabbitMqConnections(configuration, Configure);
        
        // Вызывать после вызова метода конфигурации библиотеки!
        services.AddHostedService<Service>();
    }
    
    private static void Configure(IRabbitMqConfigurator configurator)
    {
        configurator.ConfigureConnection(
            "esb",
            configureConnection: builder =>
            {
                builder.ConfigureModels(ConfigureModels);
            });
    }

    private static void ConfigureModels(IModelsBuilder builder)
    {
        builder.DeclareQueue(
            "esb-queue-direct",
            queueBuilder => queueBuilder.BindToAmqDirect("esb-queue-direct"));
    }
}