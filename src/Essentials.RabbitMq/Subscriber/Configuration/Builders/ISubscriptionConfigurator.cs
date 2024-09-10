using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Subscriber.Interception;

namespace Essentials.RabbitMq.Subscriber.Configuration.Builders;

/// <summary>
/// Билдер подписки на событие
/// </summary>
/// <typeparam name="TEvent">Тип события</typeparam>
public interface ISubscriptionConfigurator<out TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Устанавливает тип содержимого для сообщения <see cref="ContentType.Json" />
    /// </summary>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithJsonContentType();
    
    /// <summary>
    /// Устанавливает тип содержимого для сообщения <see cref="ContentType.Xml" />
    /// </summary>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithXmlContentType();
    
    /// <summary>
    /// Устанавливает количество сообщений, которое может забрать подписчик
    /// </summary>
    /// <param name="prefetchCount">Количество сообщений</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> SetRetryCount(ushort prefetchCount);
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения
    /// </summary>
    /// <typeparam name="TBehavior">Тип перехватчик обработки сообщения</typeparam>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> AttachBehavior<TBehavior>()
        where TBehavior : IMessageHandlerBehavior;
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения для сбора метрик по-умолчанию
    /// </summary>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> AttachDefaultMetricsBehavior();
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения для сбора логов по-умолчанию
    /// </summary>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> AttachDefaultLoggingBehavior();
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <typeparam name="THandler">Тип обработчика</typeparam>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler<THandler>()
        where THandler : IEventHandler<TEvent>;
    
    /// <summary>
    /// Устанавливает обработчик события по-умолчанию
    /// </summary>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithDefaultHandler();
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Action<TEvent>? handler);
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Action<IServiceProvider, TEvent>? handler);
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Func<TEvent, Task>? handler);
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Func<IServiceProvider, TEvent, Task>? handler);
}