using RabbitMQ.Client;
using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.ShareInternalServices.Publisher;

/// <summary>
/// Сервис публикации сообщений
/// </summary>
internal interface IPublisher
{
    /// <summary>
    /// Публикует сообщение
    /// </summary>
    /// <param name="channel">Канал</param>
    /// <param name="exchangeName">Название обменника</param>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <param name="body">Тело сообщения</param>
    /// <param name="properties">Свойства сообщения</param>
    /// <param name="retryCount">Количество попыток</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    Task PublishAsync(
        IModel channel,
        ExchangeName exchangeName,
        RoutingKey routingKey,
        ReadOnlyMemory<byte> body,
        IBasicProperties properties,
        int retryCount,
        CancellationToken? token = null);
}