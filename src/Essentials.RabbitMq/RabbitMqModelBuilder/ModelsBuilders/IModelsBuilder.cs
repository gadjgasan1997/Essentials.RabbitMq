using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

/// <summary>
/// Билдер модели RabbitMq
/// </summary>
public interface IModelsBuilder
{
    /// <summary>
    /// Объявляет очередь
    /// </summary>
    /// <param name="queueName">Название очереди</param>
    /// <param name="configure">Действие конфигурации</param>
    /// <returns>Билдер модели</returns>
    IModelsBuilder DeclareQueue(QueueName queueName, Action<IQueueBuilder>? configure = null);
    
    /// <summary>
    /// Объявляет обменник
    /// </summary>
    /// <param name="exchangeName">Название обменника</param>
    /// <param name="exchangeType">Тип обменника</param>
    /// <param name="configure">Действие конфигурации</param>
    /// <returns>Билдер модели</returns>
    IModelsBuilder DeclareExchange(
        ExchangeName exchangeName,
        ExchangeType exchangeType,
        Action<IExchangeBuilder>? configure = null);
}