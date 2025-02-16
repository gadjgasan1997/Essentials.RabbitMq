namespace Essentials.RabbitMq.RabbitMqModelBuilder.ModelsBuilders;

/// <summary>
/// Билдер обменника
/// </summary>
public interface IExchangeBuilder
{
    /// <summary>
    /// Указывает, что обменник сохраняет свое состояние
    /// </summary>
    /// <returns>Билдер обменника</returns>
    IExchangeBuilder Durable();
    
    /// <summary>
    /// Указывает, что обменник будет удален автоматически
    /// </summary>
    /// <returns>Билдер обменника</returns>
    IExchangeBuilder AutoDelete();
}