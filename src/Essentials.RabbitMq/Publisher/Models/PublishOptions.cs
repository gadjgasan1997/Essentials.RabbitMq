using RabbitMQ.Client;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.Publisher.Models;

/// <summary>
/// Опции публикации события
/// </summary>
internal class PublishOptions
{
    public PublishOptions(
        ContentType contentType,
        int retryCount,
        DeliveryMode deliveryMode,
        string? correlationId,
        string? replyTo,
        IReadOnlyCollection<Type> behaviors,
        IDictionary<string, object> headers,
        Action<IBasicProperties>? configureProperties = null)
    {
        ContentType = contentType;
        RetryCount = retryCount;
        DeliveryMode = deliveryMode;
        CorrelationId = correlationId;
        ReplyTo = replyTo;
        Behaviors = behaviors;
        Headers = headers;
        ConfigureProperties = configureProperties;
    }
    
    /// <summary>
    /// Тип содержимого
    /// </summary>
    public ContentType ContentType { get; }
    
    /// <summary>
    /// Количество попыток публикации сообщения
    /// </summary>
    public int RetryCount { get; }
    
    /// <summary>
    /// Режим доставки сообщения
    /// </summary>
    public DeliveryMode DeliveryMode { get; }
    
    /// <summary>
    /// Id корреляции для связи сообщений
    /// </summary>
    public string? CorrelationId { get; }
    
    /// <summary>
    /// Ключ маршрутизации, с которым должно публиковаться ответное сообщение
    /// </summary>
    public string? ReplyTo { get; }
    
    /// <summary>
    /// Список перехватчиков
    /// </summary>
    public IReadOnlyCollection<Type> Behaviors { get; }
    
    /// <summary>
    /// Заголовки
    /// </summary>
    public IDictionary<string, object> Headers { get; }
    
    /// <summary>
    /// Делегат конфигурации свойств сообщения
    /// </summary>
    public Action<IBasicProperties>? ConfigureProperties { get; }
}