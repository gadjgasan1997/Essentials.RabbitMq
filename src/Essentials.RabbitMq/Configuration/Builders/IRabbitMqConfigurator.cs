using Microsoft.Extensions.DependencyInjection;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Publisher.Interception;
using Essentials.RabbitMq.Subscriber.Interception;
using static Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace Essentials.RabbitMq.Configuration.Builders;

/// <summary>
/// Билдер конфигурации RabbitMq
/// </summary>
public interface IRabbitMqConfigurator
{
    /// <summary>
    /// Настраивает соединение
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="configureConnection">Действие конфигурации</param>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator ConfigureConnection(
        string connectionName,
        Action<IConnectionConfigurator> configureConnection);
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения
    /// </summary>
    /// <param name="lifetime">Время жизни</param>
    /// <typeparam name="TBehavior">Перехватчик</typeparam>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator AddMessageHandlerBehavior<TBehavior>(ServiceLifetime? lifetime = Singleton)
        where TBehavior : IMessageBehavior;
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения для сбора логов
    /// </summary>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator AddLoggingMessageHandlerBehavior() =>
        AddMessageHandlerBehavior<LoggingMessageHandlerBehavior>();
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения для сбора метрик
    /// </summary>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator AddMetricsMessageHandlerBehavior() =>
        AddMessageHandlerBehavior<MetricsMessageHandlerBehavior>();
    
    /// <summary>
    /// Добавляет перехватчик отправки сообщения
    /// </summary>
    /// <param name="lifetime">Время жизни</param>
    /// <typeparam name="TBehavior">Перехватчик</typeparam>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator AddMessagePublisherBehavior<TBehavior>(ServiceLifetime? lifetime = Singleton)
        where TBehavior : IMessageBehavior;
    
    /// <summary>
    /// Добавляет перехватчик отправки сообщения для сбора логов
    /// </summary>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator AddLoggingMessagePublisherBehavior() =>
        AddMessagePublisherBehavior<LoggingMessagePublisherBehavior>();
    
    /// <summary>
    /// Добавляет перехватчик отправки сообщения для сбора метрик
    /// </summary>
    /// <returns>Билдер конфигурации RabbitMq</returns>
    IRabbitMqConfigurator AddMetricsMessagePublisherBehavior() =>
        AddMessagePublisherBehavior<MetricsMessagePublisherBehavior>();
}