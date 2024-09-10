using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Essentials.RabbitMq.RpcCaller.Implementations;

namespace Essentials.RabbitMq.RpcCaller.Extensions;

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
    public static IServiceCollection ConfigureRabbitMqRpcCaller(this IServiceCollection services)
    {
        services.TryAddTransient<IRpcTasksService, RpcTasksService>();
        services.TryAddTransient<IRpcCaller, Implementations.RpcCaller>();
        services.TryAddTransient<IConnectionsService, ConnectionsService>();
        
        return services;
    }
}