using RabbitMQ.Client.Events;
using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.Subscriber.Models;

/// <summary>
/// Ключ подписки на событие
/// </summary>
public record SubscriptionKey
{
    internal const string DELIMITER = "_____";
    
    private SubscriptionKey(ConnectionName connectionName, QueueName queueName)
    {
        ConnectionName = connectionName;
        QueueName = queueName;
        Value = $"{connectionName.Value}{DELIMITER}{queueName.Value}";
    }
    
    /// <summary>
    /// Название соединения
    /// </summary>
    public ConnectionName ConnectionName { get; }
    
    /// <summary>
    /// Название очереди
    /// </summary>
    public QueueName QueueName { get; }
    
    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Создает ключ подписки
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="queueName">Название очереди</param>
    /// <returns>Ключ подписки</returns>
    public static SubscriptionKey Create(ConnectionName connectionName, QueueName queueName) =>
        new(connectionName, queueName);
    
    /// <summary>
    /// Создает ключ подписки
    /// </summary>
    /// <param name="eventArgs">Аргументы события</param>
    /// <returns>Ключ подписки</returns>
    internal static SubscriptionKey Create(BasicDeliverEventArgs eventArgs)
    {
        if (string.IsNullOrWhiteSpace(eventArgs.ConsumerTag))
            throw new ArgumentException("Не задан тег слушателя");
        
        var parts = eventArgs.ConsumerTag.Split(DELIMITER);
        if (parts.Length != 2)
        {
            throw new ArgumentException(
                $"Тег слушателя имеет неизвестный формат: '{eventArgs.ConsumerTag}'");
        }
        
        var connectionName = ConnectionName.Create(parts[0]);
        var queueName = QueueName.Create(parts[1]);
        return Create(connectionName, queueName);
    }
}