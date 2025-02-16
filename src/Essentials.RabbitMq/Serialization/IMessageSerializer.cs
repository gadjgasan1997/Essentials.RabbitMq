using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.Serialization;

/// <summary>
/// Сериалайзер сообщений
/// </summary>
internal interface IMessageSerializer
{
    /// <summary>
    /// Сериализует событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="contentType">Тип содержимого</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns>Сериализованное событие</returns>
    ReadOnlyMemory<byte> Serialize<TEvent>(TEvent @event, ContentType contentType)
        where TEvent : IEvent;
    
    /// <summary>
    /// Десериализует сообщение
    /// </summary>
    /// <param name="content">Сообщение</param>
    /// <param name="eventType">Тип события</param>
    /// <param name="contentType">Тип содержимого</param>
    /// <returns>Десериализованное событие</returns>
    IEvent Deserialize(ReadOnlyMemory<byte> content, Type eventType, ContentType contentType);
}