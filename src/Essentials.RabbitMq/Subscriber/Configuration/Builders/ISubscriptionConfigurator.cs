using Essentials.RabbitMq.Handlers;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber.Configuration.Builders;

/// <summary>
/// Билдер подписки на событие
/// </summary>
/// <typeparam name="TEvent">Тип события</typeparam>
public interface ISubscriptionConfigurator<out TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Активирует подписку на событие только при включенным флаге
    /// </summary>
    /// <param name="featureFlag">Флаг</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithFeatureFlag(string featureFlag);
    
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
    ISubscriptionConfigurator<TEvent> SetPrefetchCount(ushort prefetchCount);
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения
    /// </summary>
    /// <typeparam name="TBehavior">Тип перехватчик обработки сообщения</typeparam>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> AttachBehavior<TBehavior>()
        where TBehavior : IMessageBehavior;
    
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
    /// Обрабатывает сообщение по-условию
    /// </summary>
    /// <param name="predicate">Условие, в случае которого будет обработано сообщение</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> When(Func<string, HashSet<MessageHeader>, bool> predicate);
    
    /// <summary>
    /// Обрабатывает сообщение в случае, если его ключ маршрутизации равен требуемому
    /// </summary>
    /// <param name="routingKey">Ключ маршрутизации</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WhenRoutingKeyIs(string routingKey);
    
    /// <summary>
    /// Обрабатывает сообщение в случае, если его заголовки содержат требуемый
    /// </summary>
    /// <param name="header">Заголовок</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WhenHeadersContains(MessageHeader header);
    
    /// <summary>
    /// Обрабатывает сообщение в случае, если его заголовки содержат требуемые
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WhenHeadersContains(HashSet<MessageHeader> headers);
    
    /// <summary>
    /// Обрабатывает сообщение в случае, если его заголовки равны требуемым
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WhenHeadersAreEqual(HashSet<MessageHeader> headers);
    
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
    ISubscriptionConfigurator<TEvent> WithHandler(Action<TEvent> handler);
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Action<IServiceProvider, TEvent> handler);
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Func<TEvent, Task> handler);
    
    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="handler">Делегат обработки события</param>
    /// <returns>Билдер подписки на событие</returns>
    ISubscriptionConfigurator<TEvent> WithHandler(Func<IServiceProvider, TEvent, Task> handler);
}