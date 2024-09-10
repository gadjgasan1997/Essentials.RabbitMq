namespace Essentials.RabbitMq.Options;

/// <summary>
/// Опции Ssl соединений
/// </summary>
internal class SslOptions
{
    /// <summary>
    /// Признак необходимости использовать Ssl соединение
    /// </summary>
    public bool Enable { get; init; }
    
    /// <summary>
    /// Путь к сертификату
    /// </summary>
    public string? CertPath { get; init; }
    
    /// <summary>
    /// Пароль для сертификата
    /// </summary>
    public string? CertPassphrase { get; init; }
    
    /// <summary>
    /// Название Ssl сервера
    /// </summary>
    public string? SslServerName { get; init; }
}