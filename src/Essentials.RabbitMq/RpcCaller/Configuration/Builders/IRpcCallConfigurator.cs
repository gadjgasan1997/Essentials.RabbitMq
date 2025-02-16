using RabbitMQ.Client;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.RpcCaller.Interception;
using RabbitMQ.Client.Events;

namespace Essentials.RabbitMq.RpcCaller.Configuration.Builders;

/// <summary>
/// Билдер конфигурации Rpc запроса
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
public interface IRpcCallConfigurator<out TRequest>
    where TRequest : IRpcCallRequest
{
    /// <summary>
    /// Устанавливает тип содержимого для сообщения <see cref="ContentType.Json" />
    /// </summary>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> WithJsonContentType();
    
    /// <summary>
    /// Устанавливает тип содержимого для сообщения <see cref="ContentType.Xml" />
    /// </summary>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> WithXmlContentType();
    
    /// <summary>
    /// Устанавливает количество попыток публикации сообщения
    /// </summary>
    /// <param name="retryCount">Количество попыток</param>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> SetRetryCount(int retryCount);
    
    /// <summary>
    /// Устанавливает режим доставки сообщения
    /// </summary>
    /// <param name="deliveryMode">Режим доставки сообщения</param>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> SetDeliveryMode(DeliveryMode deliveryMode);
    
    /// <summary>
    /// Устанавливает Id корреляции для связи сообщений
    /// </summary>
    /// <param name="correlationId">Id корреляции</param>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> SetCorrelationId(string correlationId);
    
    IRpcCallConfigurator<TRequest> ReceiveCorrelationIdByFunc(Func<BasicDeliverEventArgs, string> func);
    
    /// <summary>
    /// Устанавливает таймаут ожидания ответа на запрос
    /// </summary>
    /// <param name="timeout">Таймаут</param>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> SetTimeout(TimeSpan timeout);
    
    /// <summary>
    /// Добавляет перехватчик обработки сообщения
    /// </summary>
    /// <typeparam name="TBehavior">Тип перехватчик обработки сообщения</typeparam>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> AttachBehavior<TBehavior>()
        where TBehavior : IRpcCallBehavior;
    
    /// <summary>
    /// Настраивает свойства сообщения
    /// </summary>
    /// <param name="configure">Действие конфигурации свойств</param>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> ConfigureProperties(Action<IBasicProperties> configure);
    
    /// <summary>
    /// Добавляет заголовок к сообщению
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    /// <typeparam name="TValue">Тип значения</typeparam>
    /// <returns>Билдер конфигурации Rpc запроса</returns>
    IRpcCallConfigurator<TRequest> AttachHeader<TValue>(string name, TValue value);
}