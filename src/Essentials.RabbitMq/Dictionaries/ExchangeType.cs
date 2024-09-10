namespace Essentials.RabbitMq.Dictionaries;

/// <summary>
/// Тип обменника
/// </summary>
public record ExchangeType
{
    private ExchangeType(int value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }
    
    /// <summary>
    /// Тип обменника
    /// </summary>
    public int Value { get; }
    
    /// <summary>
    /// Название типа
    /// </summary>
    public string DisplayName { get; }
    
    /// <summary>
    /// direct
    /// </summary>
    public static ExchangeType Direct => new(1, "direct");
    
    /// <summary>
    /// fanout
    /// </summary>
    public static ExchangeType Fanout => new(2, "fanout");
    
    /// <summary>
    /// headers
    /// </summary>
    public static ExchangeType Headers => new(3, "headers");
    
    /// <summary>
    /// topic
    /// </summary>
    public static ExchangeType Topic => new(4, "topic");
}