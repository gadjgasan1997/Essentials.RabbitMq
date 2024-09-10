using RabbitMQ.Client;
using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher;

/// <summary>
/// Билдер сообщения
/// </summary>
internal interface IMessageBuilder
{
    /// <summary>
    /// Пополняет свойства сообщения данными из опций публикации события
    /// </summary>
    /// <param name="properties">Свойства сообщения</param>
    /// <param name="options">Опции публикации события</param>
    /// <param name="handledMessageCorrelationId">CorrelationId с обрабатываемого сообщения</param>
    /// <returns>Свойства</returns>
    IBasicProperties EnsureProperties(
        IBasicProperties properties,
        PublishOptions options,
        string? handledMessageCorrelationId = null);
}