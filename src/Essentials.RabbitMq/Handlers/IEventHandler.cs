namespace Essentials.RabbitMq.Handlers;

/// <summary>
/// Обработчик события
/// </summary>
/// <typeparam name="TEvent">Тип события</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Обрабатывает событие
    /// </summary>
    /// <param name="event">Событие</param>
    /// <returns></returns>
    ValueTask HandleAsync(TEvent @event);
}