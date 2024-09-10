using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Essentials.RabbitMq.Publisher.Implementations;
using Essentials.RabbitMq.Publisher.Metrics.Extensions;

namespace Essentials.RabbitMq.Publisher.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает сервис для публикации событий
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureRabbitMqEventsPublisher(this IServiceCollection services)
    {
        services.TryAddTransient<IMessageBuilder, MessageBuilder>();
        services.TryAddSingleton<IConnectionsService, ConnectionsService>();
        services.TryAddTransient<IEventsPublisher, EventsPublisher>();
        services.ConfigureMetricsService();
        
        return services;
    }
}