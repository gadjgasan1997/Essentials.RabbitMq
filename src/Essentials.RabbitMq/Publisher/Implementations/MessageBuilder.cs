using RabbitMQ.Client;
using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher.Implementations;

/// <inheritdoc cref="IMessageBuilder" />
internal class MessageBuilder : IMessageBuilder
{
    /// <inheritdoc cref="IMessageBuilder.EnsureProperties" />
    public IBasicProperties EnsureProperties(
        IBasicProperties properties,
        PublishOptions options,
        string? handledMessageCorrelationId = null)
    {
        properties.DeliveryMode = (byte) options.DeliveryMode;
        properties.Headers = options.Headers;
        
        // Проставление параметра CorrelationId значением с обрабатываемого сообщения.
        // Обычно происходит, если сообщение публикуется как ответ на запрос
        if (!string.IsNullOrWhiteSpace(handledMessageCorrelationId))
            properties.CorrelationId = handledMessageCorrelationId;
        
        // Проставление параметра CorrelationId из опций публикации.
        // Обычно происходит, если публикация сообщения инициирована с целью получить на него ответ.
        // Т.е. параметр обычно проставляется тем, кто запросил ответ и ждет
        if (!string.IsNullOrWhiteSpace(options.CorrelationId))
            properties.CorrelationId = options.CorrelationId;
        
        // Проставление параметра ReplyTo из опций публикации.
        // Обычно происходит, если публикация сообщения инициирована с целью получить на него ответ.
        // Т.е. параметр обычно проставляется тем, кто запросил ответ и ждет
        if (!string.IsNullOrWhiteSpace(options.ReplyTo))
            properties.ReplyTo = options.ReplyTo;
        
        options.ConfigureProperties?.Invoke(properties);
        return properties;
    }
}