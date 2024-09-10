using System.ComponentModel.DataAnnotations;

namespace Essentials.RabbitMq.Options;

/// <summary>
/// Опции соединения с RabbitMq
/// </summary>
internal class ConnectionOptions
{
    /// <summary>
    /// Название подключения
    /// </summary>
    [Required]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Хост очереди
    /// </summary>
    [Required]
    public string Host { get; init; } = null!;
        
    /// <summary>
    /// Порт очереди
    /// </summary>
    public int Port { get; init; }
        
    /// <summary>
    /// Виртуал хост очереди
    /// </summary>
    [Required]
    public string VirtualHost { get; init; } = null!;
        
    /// <summary>
    /// Логин для подключения
    /// </summary>
    [Required]
    public string UserName { get; init; } = null!;

    /// <summary>
    /// Пароль для подключения
    /// </summary>
    [Required]
    public string Password { get; init; } = null!;
        
    /// <summary>
    /// Количество попыток на подключение к очереди
    /// </summary>
    public int? ConnectRetryCount { get; init; }

    /// <summary>
    /// Признак, что слушатели должны быть обработаны асинхронно
    /// </summary>
    public bool DispatchConsumersAsync { get; init; }

    /// <summary>
    /// Опции Ssl соединения
    /// </summary>
    public SslOptions Ssl { get; init; } = new();
}