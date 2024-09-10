using System.Text;
using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Subscriber.Models;

/// <summary>
/// Заголовок сообщения
/// </summary>
public record MessageHeader
{
    private MessageHeader(string name, object? value = null)
    {
        Name = name;
        Value = value;
    }
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Значение
    /// </summary>
    public object? Value { get; }
    
    /// <summary>
    /// Создает заголовок сообщения
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="value">Значение</param>
    /// <returns>Заголовок сообщения</returns>
    public static MessageHeader Create(string name, object? value = null)
    {
        name.CheckNotNullOrEmpty("Название заголовка не может быть пустым");
        return new MessageHeader(name, value);
    }
    
    /// <summary>
    /// Создает заголовок сообщения
    /// </summary>
    /// <param name="pair">Название/значение</param>
    /// <returns>Заголовок сообщения</returns>
    internal static MessageHeader Create(KeyValuePair<string, object?> pair)
    {
        var value = pair.Value switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            _ => pair.Value
        };
        
        return Create(pair.Key, value);
    }
    
    /// <summary>
    /// Создает заголовок сообщения
    /// </summary>
    /// <param name="tuple">Кортеж с названием и значением</param>
    /// <returns>Заголовок сообщения</returns>
    public static implicit operator MessageHeader((string, object?) tuple) => Create(tuple.Item1, tuple.Item2);
}