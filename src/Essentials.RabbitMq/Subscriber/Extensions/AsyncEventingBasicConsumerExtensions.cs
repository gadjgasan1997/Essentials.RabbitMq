using RabbitMQ.Client.Events;

namespace Essentials.RabbitMq.Subscriber.Extensions;

/// <summary>
/// Методы расширения для <see cref="AsyncEventingBasicConsumer" />
/// </summary>
internal static class AsyncEventingBasicConsumerExtensions
{
    /// <summary>
    /// Возвращает строку с тегами слушателя
    /// </summary>
    /// <param name="consumer">Слушатель</param>
    /// <returns>Строка с тегами слушателя</returns>
    public static string GetConsumerTagsString(this AsyncEventingBasicConsumer consumer) =>
        string.Join(", ", consumer.ConsumerTags);
}