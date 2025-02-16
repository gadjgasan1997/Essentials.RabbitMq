using Essentials.Serialization;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Dictionaries;

namespace Essentials.RabbitMq.Serialization;

/// <inheritdoc cref="IMessageSerializer" />
internal class MessageSerializer : IMessageSerializer
{
    /// <inheritdoc cref="IMessageSerializer.Serialize{TEvent}" />
    public ReadOnlyMemory<byte> Serialize<TEvent>(TEvent @event, ContentType contentType)
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
    
    /// <inheritdoc cref="IMessageSerializer.Deserialize" />
    public IEvent Deserialize(ReadOnlyMemory<byte> content, Type eventType, ContentType contentType)
    {
        var deserializer = contentType switch
        {
            ContentType.Json => EssentialsDeserializersFactory.TryGet(KnownRabbitMqDeserializers.JSON),
            ContentType.Xml => EssentialsDeserializersFactory.TryGet(KnownRabbitMqDeserializers.XML),
            _ => throw new KeyNotFoundException($"Не найден десериалайзер для типа содержимого '{contentType}'")
        };
        
        var result = deserializer.Deserialize(eventType, content);
        result.CheckNotNull("Результат после десериализации сообщения равен null");
        
        if (result is not IEvent @event)
        {
            throw new InvalidOperationException(
                $"Результат после десериализации не соответствует интерфейсу '{typeof(IEvent).FullName}'");
        }
        
        return @event;
    }
}