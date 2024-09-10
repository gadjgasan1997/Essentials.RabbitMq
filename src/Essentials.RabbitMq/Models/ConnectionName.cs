using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Название соединения
/// </summary>
internal record ConnectionName
{
    public ConnectionName(string value)
    {
        Value = value
            .CheckNotNullOrEmpty()
            .FullTrim()!
            .ToLowerInvariant();
    }
    
    /// <summary>
    /// Название соединения
    /// </summary>
    public string Value { get; }
}