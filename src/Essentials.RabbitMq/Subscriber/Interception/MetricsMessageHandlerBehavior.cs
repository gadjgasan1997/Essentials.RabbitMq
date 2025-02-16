using Essentials.RabbitMq.Interception;

namespace Essentials.RabbitMq.Subscriber.Interception;

/// <summary>
/// Перехватчик обработки сообщения для сбора метрик
/// </summary>
public class MetricsMessageHandlerBehavior : IMessageBehavior
{
    /// <inheritdoc cref="IMessageBehavior.Handle" />
    public async Task Handle(NextActionDelegate next)
    {
        await next();
    }
}