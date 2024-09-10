using Microsoft.Extensions.DependencyInjection;
using Essentials.RabbitMq.Publisher.Metrics.Implementations;

namespace Essentials.RabbitMq.Publisher.Metrics.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает сервис управления метриками
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureMetricsService(
        this IServiceCollection services)
    {
        // todo реализовать отдачу метрик
        return services.ConfigureMockMetrics();
    }

    /// <summary>
    /// Настраивает мок для метрик
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection ConfigureMockMetrics(this IServiceCollection services)
    {
        services.AddSingleton<IMetricsService, MockMetricsService>();
        return services;
    }
}