namespace Essentials.RabbitMq.Dictionaries;

/// <summary>
/// Известные сериалайзеры, использующиеся для RabbitMq
/// </summary>
public static class KnownRabbitMqSerializers
{
    /// <summary>
    /// Json
    /// </summary>
    public const string JSON = "RabbitMqJsonSerializer";
    
    /// <summary>
    /// Xml
    /// </summary>
    public const string XML = "RabbitMqXmlSerializer";
}