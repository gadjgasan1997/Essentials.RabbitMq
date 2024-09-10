using RabbitMQ.Client;
using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.Publisher;

/// <summary>
/// Сервис для работы с соединениями
/// </summary>
internal interface IConnectionsService
{
    /// <summary>
    /// Создает или возвращает существующий канал
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <returns>Канал</returns>
    IModel GetOrCreateChannel(ConnectionName connectionName);
}