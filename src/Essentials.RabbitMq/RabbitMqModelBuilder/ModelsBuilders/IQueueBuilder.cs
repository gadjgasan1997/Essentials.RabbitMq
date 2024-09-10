using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

/// <summary>
/// Билдер очереди
/// </summary>
public interface IQueueBuilder
{
    /// <summary>
    /// Указывает, что очередь сохраняет свое состояние
    /// </summary>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder Durable();
    
    /// <summary>
    /// Указывает, что очередь разрешает подключаться только одному потребителю
    /// </summary>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder Exclusive();
    
    /// <summary>
    /// Указывает, что очередь будет удалена автоматически
    /// </summary>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder AutoDelete();
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику
    /// </summary>
    /// <param name="exchangeName">Название обменника</param>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder Bind(
        ExchangeName exchangeName,
        RoutingKey routingKey,
        IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику с ключом маршрутизации по-умолчанию
    /// </summary>
    /// <param name="exchangeName">Название обменника</param>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindDefault(
        ExchangeName exchangeName,
        IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику <see cref="KnownExchanges.AMQ_DIRECT" />
    /// </summary>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindToAmqDirect(
        RoutingKey routingKey,
        IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику <see cref="KnownExchanges.AMQ_DIRECT" /> с ключом маршрутизации по-умолчанию
    /// </summary>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindToAmqDirectDefault(IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику <see cref="KnownExchanges.AMQ_FANOUT" />
    /// </summary>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindToFanoutDirect(IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику <see cref="KnownExchanges.AMQ_FANOUT" /> с ключом маршрутизации по-умолчанию
    /// </summary>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindToFanoutDirectDefault(IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику <see cref="KnownExchanges.AMQ_TOPIC" />
    /// </summary>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindToAmqTopic(
        RoutingKey routingKey,
        IDictionary<string, object>? arguments = null);
    
    /// <summary>
    /// Выполняет привязку очереди к обменнику <see cref="KnownExchanges.AMQ_TOPIC" /> с ключом маршрутизации по-умолчанию
    /// </summary>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Билдер очереди</returns>
    IQueueBuilder BindToAmqTopicDefault(IDictionary<string, object>? arguments = null);
}