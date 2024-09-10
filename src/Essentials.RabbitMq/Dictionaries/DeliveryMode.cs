namespace Essentials.RabbitMq.Dictionaries;

/// <summary>
/// Режим доставки сообщения
/// </summary>
public enum DeliveryMode
{
    /// <summary>
    /// Не хранится на диске
    /// </summary>
    NonPersistent = 1,
    
    /// <summary>
    /// Хранится на диске
    /// </summary>
    Persistent = 2
}