namespace Essentials.RabbitMq.Subscriber.Interception.Behaviors;

/// <summary>
/// Перехватчик обработки сообщения для сбора метрик
/// </summary>
public class MetricsMessageHandlerBehavior : IMessageHandlerBehavior
{
    public async Task Handle(MessageHandlerDelegate next)
    {
        await next();
    }
}