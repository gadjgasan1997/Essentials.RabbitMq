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
        List<Binding> bindings,
        IDictionary<string, object>? arguments = null)
    {
        Name = name;
        Durable = durable;
        Exclusive = exclusive;
        AutoDelete = autoDelete;
        _bindings = bindings;
        Arguments = arguments ?? new Dictionary<string, object>();
    }
    
    private readonly List<Binding> _bindings;
    
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
    public IReadOnlyCollection<Binding> Bindings => _bindings.AsReadOnly();
    
    /// <summary>
    /// Аргументы
    /// </summary>
    public IDictionary<string, object> Arguments { get; }

    /// <summary>
    /// Добавляет привязки
    /// </summary>
    /// <param name="bindings">Привязки</param>
    public void AddBindings(IEnumerable<Binding> bindings)
    {
        var newBindings = bindings
            .Where(binding => !_bindings.Contains(binding, BindingEqualityComparer.Instance))
            .ToList();
        
        _bindings.AddRange(newBindings);
    }
}