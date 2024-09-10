namespace Essentials.RabbitMq.Options;

/// <summary>
/// Опции соединений
/// </summary>
internal class ConnectionsOptions
{
    public static string Section => "RabbitMqOptions";
    
    /// <summary>
    /// Опции соединений
    /// </summary>
    public List<ConnectionOptions> Connections { get; set; } = [];
}