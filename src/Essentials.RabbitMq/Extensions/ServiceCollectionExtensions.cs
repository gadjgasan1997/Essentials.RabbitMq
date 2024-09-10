using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Essentials.Serialization.Serializers;
using Essentials.Serialization.Deserializers;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Configuration.Builders;
using Essentials.RabbitMq.Publisher.Extensions;
using Essentials.RabbitMq.RabbitMqConnections.Extensions;
using Essentials.RabbitMq.RabbitMqModelBuilder.Extensions;
using Essentials.RabbitMq.RpcCaller.Extensions;
using Essentials.RabbitMq.Subscriber.Extensions;
using static Essentials.Serialization.EssentialsDeserializersFactory;
using static Essentials.Serialization.EssentialsSerializersFactory;

namespace Essentials.RabbitMq.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureRabbitMqConnections(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRabbitMqConfigurator>? configureAction = null)
    {
        var rabbitMqBuilder = new RabbitMqConfigurator()
            .AddLoggingMessageHandlerBehavior()
            .AddMetricsMessageHandlerBehavior()
            .AddLoggingMessagePublisherBehavior()
            .AddMetricsMessagePublisherBehavior();
        
        configureAction?.Invoke(rabbitMqBuilder);
        
        RabbitMqConfigurator.RegisterBehaviors(services);
        
        AddByKey(KnownRabbitMqDeserializers.JSON, () => new NewtonsoftJsonDeserializer());
        AddByKey(KnownRabbitMqDeserializers.XML, () => new XmlDeserializer());
        AddByKey(KnownRabbitMqSerializers.JSON, () => new NewtonsoftJsonSerializer());
        AddByKey(KnownRabbitMqSerializers.XML, () => new XmlSerializer());

        services
            .ConfigureRabbitMqConnectionFactory(configuration)
            .ConfigureRabbitMqModelsBuilders()
            .ConfigureRabbitMqEventsPublisher()
            .ConfigureRabbitMqEventsSubscriber()
            .ConfigureRabbitMqRpcCaller();
        
        return services;
    }
}