using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RabbitMqConnections;

/// <summary>
/// Фабрика для получения соединений с RabbitMq
/// </summary>
internal interface IRabbitMqConnectionFactory : IDisposable
{
    /// <summary>
    /// Возвращает соединение
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <returns>Соединение</returns>
    IRabbitMqConnection GetConnection(ConnectionName connectionName);
}