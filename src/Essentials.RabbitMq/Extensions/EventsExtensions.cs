using Essentials.Serialization;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.Extensions;

/// <summary>
/// Методы расширения для событий
/// </summary>
internal static class EventsExtensions
{
    /// <summary>
    /// Создает сообщение, сериализуя события <seealso cref="@event" />
    /// </summary>
    /// <param name="event">Событие</param>
    /// <param name="contentType">Тип содержимого сообщения</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns>Сообщение</returns>
    public static ReadOnlyMemory<byte> CreateMessage<TEvent>(this TEvent @event, ContentType contentType)
        where TEvent : IEvent
    {
        var serializer = contentType switch
        {
            ContentType.Json => EssentialsSerializersFactory.TryGet(KnownRabbitMqSerializers.JSON),
            ContentType.Xml => EssentialsSerializersFactory.TryGet(KnownRabbitMqSerializers.XML),
            _ => throw new KeyNotFoundException($"Не найден сериалайзер для типа содержимого '{contentType}'")
        };
        
        return serializer.Serialize(@event);
    }
}