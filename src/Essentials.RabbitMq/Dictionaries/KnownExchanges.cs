namespace Essentials.RabbitMq.Dictionaries;

/// <summary>
/// Известные обменники
/// </summary>
public static class KnownExchanges
{
    /// <summary>
    /// amq.direct
    /// </summary>
    public const string AMQ_DIRECT = "amq.direct";
    
    /// <summary>
    /// amq.fanout
    /// </summary>
    public const string AMQ_FANOUT = "amq.fanout";
    
    /// <summary>
    /// amq.headers
    /// </summary>
    public const string AMQ_HEADERS = "amq.headers";
    
    /// <summary>
    /// amq.topic
    /// </summary>
    public const string AMQ_TOPIC = "amq.topic";
}