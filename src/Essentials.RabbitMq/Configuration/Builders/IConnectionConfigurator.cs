using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Publisher.Configuration.Builders;
using Essentials.RabbitMq.Subscriber.Configuration.Builders;
using Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

namespace Essentials.RabbitMq.Configuration.Builders;

/// <summary>
/// Билдер соединения
/// </summary>
public interface IConnectionConfigurator
{
    /// <summary>
    /// Настраивает модели RabbitMq
    /// </summary>
    /// <param name="configure">Действие конфигурации</param>
    /// <returns>Билдер соединения</returns>
    IConnectionConfigurator ConfigureModels(Action<IModelsBuilder> configure);
    
    /// <summary>
    /// Настраивает публикацию события
    /// </summary>
    /// <param name="exchangeName">Название обменника</param>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="configure">Действие конфигурации</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns>Билдер соединения</returns>
    IConnectionConfigurator ConfigurePublish<TEvent>(
        ExchangeName exchangeName,
        RoutingKey routingKey,
        Action<IPublishConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent;
    
    /// <summary>
    /// Подписывается на событие
    /// </summary>
    /// <param name="queueName">Название очереди</param>
    /// <param name="configureQueue">Действие конфигурации очереди</param>
    /// <param name="configureSubscription">Действие конфигурации подписки</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns>Билдер соединения</returns>
    IConnectionConfigurator SubscribeOn<TEvent>(
        QueueName queueName,
        Action<IQueueBuilder>? configureQueue = null,
        Action<ISubscriptionConfigurator<TEvent>>? configureSubscription = null)
        where TEvent : IEvent;
}