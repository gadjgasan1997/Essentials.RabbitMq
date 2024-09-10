using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Название обменника
/// </summary>
internal record ExchangeName
{
    public ExchangeName(string value)
    {
        Value = value
            .CheckNotNullOrEmpty()
            .FullTrim()!
            .ToLowerInvariant();
    }
    
    /// <summary>
    /// Название обменника
    /// </summary>
    public string Value { get; }
}