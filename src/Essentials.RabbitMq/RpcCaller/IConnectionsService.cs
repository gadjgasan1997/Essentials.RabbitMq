using RabbitMQ.Client;
using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RpcCaller;

/// <summary>
/// Сервис для работы с соединениями
/// </summary>
internal interface IConnectionsService
{
    /// <summary>
    /// Создает или возвращает существующий канал для публикации события
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <returns>Канал</returns>
    IModel GetOrCreateChannelForPublish(ConnectionName connectionName);
    
    IModel GetOrCreateChannelForConsume(RoutingKey replyTo);
}