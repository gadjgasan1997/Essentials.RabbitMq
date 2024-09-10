using Essentials.RabbitMq.Dictionaries;
using static Essentials.RabbitMq.Dictionaries.KnownExchanges;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Обменник
/// </summary>
internal record Exchange
{
    public Exchange(
        ExchangeName name,
        ExchangeType type,
        bool? durable,
        bool autoDelete,
        IDictionary<string, object>? arguments = null)
    {
        Name = name;
        Type = type;
        Durable = durable ?? true;
        AutoDelete = autoDelete;
        Arguments = arguments ?? new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Название обменника
    /// </summary>
    public ExchangeName Name { get; }
    
    /// <summary>
    /// Тип обменника
    /// </summary>
    public ExchangeType Type { get; }
    
    /// <summary>
    /// Признак, что обменник сохраняет свое состояние
    /// </summary>
    public bool Durable { get; }
    
    /// <summary>
    /// Признак, что обменник будет удален автоматически
    /// </summary>
    public bool AutoDelete { get; }
    
    /// <summary>
    /// Аргументы
    /// </summary>
    public IDictionary<string, object> Arguments { get; }
    
    /// <summary>
    /// Обменник amq.direct
    /// </summary>
    public static Exchange AmqDirect => new(AMQ_DIRECT, ExchangeType.Direct, true, false);
    
    /// <summary>
    /// Обменник amq.fanout
    /// </summary>
    public static Exchange AmqFanout => new(AMQ_FANOUT, ExchangeType.Fanout, true, false);

    /// <summary>
    /// Обменник amq.headers
    /// </summary>
    public static Exchange AmqHeaders => new(AMQ_HEADERS, ExchangeType.Headers, true, false);
    
    /// <summary>
    /// Обменник amq.topic
    /// </summary>
    public static Exchange AmqTopic => new(AMQ_TOPIC, ExchangeType.Topic, true, false);
}