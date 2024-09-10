using LanguageExt;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber;

/// <summary>
/// Сервис для работы с соединениями
/// </summary>
internal interface IConnectionsService
{
    /// <summary>
    /// Создает или возвращает существующий канал
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <returns>Канал</returns>
    IModel GetOrCreateChannel(InternalSubscriptionKey key, SubscriptionOptions options);
    
    /// <summary>
    /// Создает слушателя
    /// </summary>
    /// <param name="channel">Канал</param>
    /// <param name="key">Ключ подписки на событие</param>
    /// <returns></returns>
    AsyncEventingBasicConsumer CreateConsumer(IModel channel, InternalSubscriptionKey key);
    
    /// <summary>
    /// Возвращает ключ подписки на событие
    /// </summary>
    /// <param name="consumerTags">Теги слушателя</param>
    /// <returns>Ключ подписки на событие</returns>
    Option<InternalSubscriptionKey> GetSubscriptionKey(string[] consumerTags);
}