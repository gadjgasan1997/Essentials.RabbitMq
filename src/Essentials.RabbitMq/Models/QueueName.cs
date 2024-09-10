using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.Models;

/// <summary>
/// Название очереди
/// </summary>
internal record QueueName
{
    // todo parameterized
    public QueueName(string value)
    {
        Value = value.CheckNotNullOrEmpty().FullTrim()!;
    }
    
    /// <summary>
    /// Название очереди
    /// </summary>
    public string Value { get; }
}