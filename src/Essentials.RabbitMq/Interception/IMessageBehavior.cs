namespace Essentials.RabbitMq.Interception;

/// <summary>
/// Делегат обработки сообщения
/// </summary>
public delegate Task NextActionDelegate();

/// <summary>
/// Перехватчик сообщения
/// </summary>
public interface IMessageBehavior
{
    /// <summary>
    /// Обрабатывает сообщение
    /// </summary>
    /// <param name="next">Делегат вызова следующего перехватчика или осуществления конечного действия</param>
    /// <returns>Задача</returns>
    Task Handle(NextActionDelegate next);
}