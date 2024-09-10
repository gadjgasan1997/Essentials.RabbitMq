using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.Subscriber.Models;

/// <summary>
/// Опции подписки на событие
/// </summary>
internal class SubscriptionOptions
{
    public SubscriptionOptions(
        Type eventType,
        ContentType contentType,
        ushort prefetchCount,
        IEnumerable<Type> behaviors,
        Func<IServiceProvider, IEvent, Task>? handler = null,
        Type? handlerToRegister = null,
        Func<string, HashSet<MessageHeader>, bool>? whenPredicate = null)
    {
        EventType = eventType;
        ContentType = contentType;
        PrefetchCount = prefetchCount;
        Behaviors = behaviors;
        Handler = handler;
        HandlerToRegister = handlerToRegister;
        WhenPredicate = whenPredicate;
    }
    
    /// <summary>
    /// Тип события
    /// </summary>
    public Type EventType { get; }
    
    /// <summary>
    /// Тип содержимого
    /// </summary>
    public ContentType ContentType { get; }
    
    /// <summary>
    /// Количество сообщений, которое может забрать подписчик
    /// </summary>
    public ushort PrefetchCount { get; }
    
    /// <summary>
    /// Список перехватчиков
    /// </summary>
    public IEnumerable<Type> Behaviors { get; }
    
    /// <summary>
    /// Обработчик события
    /// </summary>
    public Func<IServiceProvider, IEvent, Task>? Handler { get; private set; }
    
    /// <summary>
    /// Обработчик события для автоматической регистрации
    /// </summary>
    public Type? HandlerToRegister { get; }
    
    /// <summary>
    /// Условие, при выполнении которого сообщение будет обработано
    /// </summary>
    public Func<string, HashSet<MessageHeader>, bool>? WhenPredicate { get; }
    
    /// <summary>
    /// Устанавливает обработчик по-умолчанию
    /// </summary>
    /// <param name="handler">Обработчик</param>
    public void SetDefaultHandler(Func<IServiceProvider, IEvent, Task> handler)
    {
        if (Handler is not null)
            return;
        
        Handler = handler;
    }
}