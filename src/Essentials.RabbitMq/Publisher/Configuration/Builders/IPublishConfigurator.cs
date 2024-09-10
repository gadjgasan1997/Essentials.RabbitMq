using RabbitMQ.Client;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Interception;

namespace Essentials.RabbitMq.Publisher.Configuration.Builders;

/// <summary>
/// Билдер конфигурации публикации события
/// </summary>
/// <typeparam name="TEvent">Тип события</typeparam>
public interface IPublishConfigurator<out TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Устанавливает тип содержимого для сообщения <see cref="ContentType.Json" />
    /// </summary>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> WithJsonContentType();
    
    /// <summary>
    /// Устанавливает тип содержимого для сообщения <see cref="ContentType.Xml" />
    /// </summary>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> WithXmlContentType();
    
    /// <summary>
    /// Устанавливает количество попыток публикации сообщения
    /// </summary>
    /// <param name="retryCount">Количество попыток</param>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> SetRetryCount(int retryCount);
    
    /// <summary>
    /// Устанавливает режим доставки сообщения
    /// </summary>
    /// <param name="deliveryMode">Режим доставки сообщения</param>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> SetDeliveryMode(DeliveryMode deliveryMode);
    
    /// <summary>
    /// Устанавливает Id корреляции для связи сообщений
    /// </summary>
    /// <param name="correlationId">Id корреляции</param>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> SetCorrelationId(string correlationId);
    
    /// <summary>
    /// Устанавливает параметр с ключом маршрутизации, с которым должно публиковаться ответное сообщение
    /// </summary>
    /// <param name="replyTo">Ключ маршрутизации</param>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> SetReplyTo(string replyTo);
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения
    /// </summary>
    /// <typeparam name="TBehavior">Тип перехватчик обработки сообщения</typeparam>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> AttachBehavior<TBehavior>()
        where TBehavior : IMessageBehavior;
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения для сбора метрик по-умолчанию
    /// </summary>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> AttachDefaultMetricsBehavior();
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения для сбора логов по-умолчанию
    /// </summary>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> AttachDefaultLoggingBehavior();
    
    /// <summary>
    /// Настраивает свойства сообщения
    /// </summary>
    /// <param name="configure">Действие конфигурации свойств</param>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> ConfigureProperties(Action<IBasicProperties> configure);
    
    /// <summary>
    /// Добавляет заголовок к сообщению
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Билдер конфигурации публикации события</returns>
    IPublishConfigurator<TEvent> AttachHeader<TValue>(string name, TValue value);
}