using Essentials.RabbitMq.Options;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Essentials.RabbitMq.Subscriber.Configuration;
using Essentials.RabbitMq.Subscriber.Implementations;

namespace Essentials.RabbitMq.Subscriber.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает сервис для подписки на события
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureRabbitMqEventsSubscriber(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var flagsSection = configuration.GetSection(ConnectionsOptions.Section).GetSection("FeatureFlags");
        services.AddFeatureManagement(flagsSection);
        
        services.TryAddSingleton<IConnectionsService, ConnectionsService>();
        services.TryAddTransient<IEventsHandlerService, EventsHandlerService>();
        services.TryAddTransient<IEventsSubscriber, EventsSubscriber>();
        services.TryAddTransient<InternalEventsSubscriber>();
        services.AddHostedService<AutoSubscribeHostedService>();

        foreach (var (_, _, options) in new Storage())
        {
            if (options.HandlerToRegister is { } type)
                services.TryAddTransient(type);
        }
        
        return services;
    }
}