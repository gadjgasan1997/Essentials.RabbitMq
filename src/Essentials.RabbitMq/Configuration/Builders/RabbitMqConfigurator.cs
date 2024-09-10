using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Publisher.Interception;
using Essentials.RabbitMq.Subscriber.Interception;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace Essentials.RabbitMq.Configuration.Builders;

/// <inheritdoc cref="IRabbitMqConfigurator" />
internal class RabbitMqConfigurator : IRabbitMqConfigurator
{
    private static readonly List<ServiceDescriptor> _messageHandlerBehaviors = [];
    private static readonly List<ServiceDescriptor> _messagePublisherBehaviors = [];
    
    /// <inheritdoc cref="IRabbitMqConfigurator.ConfigureConnection" />
    public IRabbitMqConfigurator ConfigureConnection(
        string connectionName,
        Action<IConnectionConfigurator> configureConnection)
    {
        var name = ConnectionName.Create(connectionName);
        
        configureConnection.CheckNotNull("Делегат конфигурации соединения RabbitMq не может быть пустым");
        configureConnection(new ConnectionConfigurator(name));
        
        return this;
    }
    
    /// <inheritdoc cref="IRabbitMqConfigurator.AddMessageHandlerBehavior{TBehavior}" />
    public IRabbitMqConfigurator AddMessageHandlerBehavior<TBehavior>(ServiceLifetime? lifetime = Singleton)
        where TBehavior : IMessageBehavior
    {
        var behaviorLifetime = lifetime ?? Singleton;
        
        var implementationType = typeof(TBehavior);
        var descriptor = new ServiceDescriptor(typeof(IMessageBehavior), implementationType, behaviorLifetime);
        
        _messageHandlerBehaviors.Add(descriptor);
        return this;
    }
    
    /// <inheritdoc cref="IRabbitMqConfigurator.AddLoggingMessageHandlerBehavior" />    
    public IRabbitMqConfigurator AddLoggingMessageHandlerBehavior() =>
        AddMessageHandlerBehavior<LoggingMessageHandlerBehavior>();
    
    /// <inheritdoc cref="IRabbitMqConfigurator.AddMetricsMessageHandlerBehavior" />
    public IRabbitMqConfigurator AddMetricsMessageHandlerBehavior() =>
        AddMessageHandlerBehavior<MetricsMessageHandlerBehavior>();
    
    /// <inheritdoc cref="IRabbitMqConfigurator.AddMessagePublisherBehavior{TBehavior}" />
    public IRabbitMqConfigurator AddMessagePublisherBehavior<TBehavior>(ServiceLifetime? lifetime = Singleton)
        where TBehavior : IMessageBehavior
    {
        var behaviorLifetime = lifetime ?? Singleton;
        
        var implementationType = typeof(TBehavior);
        var descriptor = new ServiceDescriptor(typeof(IMessageBehavior), implementationType, behaviorLifetime);
        
        _messagePublisherBehaviors.Add(descriptor);
        return this;
    }
    
    /// <inheritdoc cref="IRabbitMqConfigurator.AddLoggingMessagePublisherBehavior" />
    public IRabbitMqConfigurator AddLoggingMessagePublisherBehavior() =>
        AddMessagePublisherBehavior<LoggingMessagePublisherBehavior>();
    
    /// <inheritdoc cref="IRabbitMqConfigurator.AddMetricsMessagePublisherBehavior" />
    public IRabbitMqConfigurator AddMetricsMessagePublisherBehavior() =>
        AddMessagePublisherBehavior<MetricsMessagePublisherBehavior>();
    
    /// <summary>
    /// Регистрирует все перехватчики сообщений
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterBehaviors(IServiceCollection services)
    {
        foreach (var serviceDescriptor in _messageHandlerBehaviors)
            services.TryAddEnumerable(serviceDescriptor);
        
        foreach (var serviceDescriptor in _messagePublisherBehaviors)
            services.TryAddEnumerable(serviceDescriptor);
    }
}