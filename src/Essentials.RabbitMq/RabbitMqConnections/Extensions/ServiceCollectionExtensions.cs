using Essentials.RabbitMq.Options;
using Essentials.RabbitMq.Extensions;
using Essentials.RabbitMq.RabbitMqConnections.Implementations;
using Essentials.Utils.Reflection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Essentials.RabbitMq.RabbitMqConnections.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает фабрику для получения соединений с RabbitMq
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureRabbitMqConnectionFactory(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = services.BuildServiceProvider();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var connectionFactory = new RabbitMqConnectionFactory(loggerFactory);
        var connectionsOptions = configuration.GetConnectionsOptions();
        
        foreach (var connectionOptions in connectionsOptions.Connections)
            connectionFactory.AddConnection(connectionOptions);
        
        services.TryAddSingleton<IRabbitMqConnectionFactory>(_ => connectionFactory);
        return services;
    }
    
    private static ConnectionsOptions GetConnectionsOptions(this IConfiguration configuration)
    {
        var section = configuration.GetSection(ConnectionsOptions.Section);
        if (!section.Exists())
            throw new InvalidOperationException("Не найдена секция с настройками соединений RabbitMq");
        
        var connectionsOptions = new ConnectionsOptions();
        section.Bind(connectionsOptions);
        
        foreach (var connectionOptions in connectionsOptions.Connections)
        {
            if (!connectionOptions.CheckRequiredProperties(out var emptyProperties))
            {
                throw new InvalidOperationException(
                    $"В конфигурации не заполнены обязательные свойства соединения: '{emptyProperties.GetNames()}'");
            }
        }

        return connectionsOptions;
    }
}