namespace Essentials.RabbitMq.Interception.Extensions;

/// <summary>
/// Методы расширения для <see cref="IMessageBehavior" />
/// </summary>
internal static class MessageBehaviorExtensions
{
    /// <summary>
    /// Вызывает список перехватчиков сообщений
    /// </summary>
    /// <param name="behaviors">Список перехватчиков</param>
    /// <param name="func">Действие, которое требуется выполнить после того, как отработают перехватчики</param>
    public static async Task InvokeAsync(
        this IEnumerable<IMessageBehavior> behaviors,
        Func<Task> func)
    {
        await behaviors
            .Aggregate<IMessageBehavior, NextActionDelegate>(
                seed: async () => await func(),
                func: (next, behavior) => async () => await behavior.Handle(next))
            .Invoke();
    }
}