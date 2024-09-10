using RabbitMQ.Client;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.RpcCaller.Models;

/// <summary>
/// Опции Rpc запроса
/// </summary>
internal class RpcCallOptions
{
    public RpcCallOptions(
        ContentType contentType,
        int retryCount,
        DeliveryMode deliveryMode,
        string correlationId,
        TimeSpan timeout,
        IReadOnlyCollection<Type> behaviors,
        IDictionary<string, object> headers,
        Action<IBasicProperties>? configureProperties)
    {
        ContentType = contentType;
        RetryCount = retryCount;
        DeliveryMode = deliveryMode;
        CorrelationId = correlationId;
        Timeout = timeout;
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
    public string CorrelationId { get; }
    
    /// <summary>
    /// Таймаут времени ожидания ответа на запрос
    /// </summary>
    public TimeSpan Timeout { get; set; }
    
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