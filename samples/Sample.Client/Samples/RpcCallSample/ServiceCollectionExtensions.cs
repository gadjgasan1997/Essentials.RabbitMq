using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.Configuration.Builders;
using Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

namespace Sample.Client.Samples.RpcCallSample;

public static class ServiceCollectionExtensions
{
    public static void ConfigureRpcCallSample(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<RpcCallSampleService>();
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

    private static void ConfigureModels(IModelsBuilder modelsBuilder)
    {
        modelsBuilder
            .DeclareQueue(
                $"test-rpc-response-queue-{Environment.GetEnvironmentVariable("APPLICATION_INSTANCE")}",
                configure: builder =>
                    builder
                        .Exclusive()
                        .AutoDelete()
                        .BindToAmqDirectDefault());
    }
}