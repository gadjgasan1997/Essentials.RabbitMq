using RabbitMQ.Client;

namespace Essentials.RabbitMq.RabbitMqConnections;

/// <summary>
/// Соединение
/// </summary>
internal interface IRabbitMqConnection : IDisposable
{
    /// <summary>
    /// Создает канал
    /// </summary>
    /// <returns>Канал</returns>
    IModel CreateModel();
}