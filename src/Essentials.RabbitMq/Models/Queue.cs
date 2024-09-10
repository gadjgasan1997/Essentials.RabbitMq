namespace Essentials.RabbitMq.Models;

/// <summary>
/// Очередь
/// </summary>
internal record Queue
{
    public Queue(
        QueueName name,
        bool durable,
        bool exclusive,
        bool autoDelete,
        IReadOnlyCollection<Binding> bindings,
        IDictionary<string, object>? arguments = null)
    {
        Name = name;
        Durable = durable;
        Exclusive = exclusive;
        AutoDelete = autoDelete;
        Bindings = bindings;
        Arguments = arguments ?? new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Название очереди
    /// </summary>
    public QueueName Name { get; }
    
    /// <summary>
    /// Признак, что очередь сохраняет свое состояние
    /// </summary>
    public bool Durable { get; }
    
    /// <summary>
    /// Признак, что очередь разрешает подключаться только одному потребителю
    /// </summary>
    public bool Exclusive { get; }
    
    /// <summary>
    /// Признак, что очередь будет удалена автоматически
    /// </summary>
    public bool AutoDelete { get; }
    
    /// <summary>
    /// Привязки к обменникам
    /// </summary>
    public IReadOnlyCollection<Binding> Bindings { get; }
    
    /// <summary>
    /// Аргументы
    /// </summary>
    public IDictionary<string, object> Arguments { get; }
}