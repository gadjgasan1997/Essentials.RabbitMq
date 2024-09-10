using RabbitMQ.Client.Events;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber.Models;

namespace Essentials.RabbitMq.Subscriber;

/// <summary>
/// Контекст обрабатываемого сообщения
/// </summary>
public static class MessageContext
{
    private static readonly AsyncLocal<Context> _current = new();
    
    /// <summary>
    /// Контекст текущего сообщения
    /// </summary>
    public static Context? Current
    {
        get => _current.Value;
        private set => _current.Value = value.CheckNotNull();
    }
    
    /// <summary>
    /// Создает контекст сообщения
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="eventArgs">Аргументы события</param>
    internal static void CreateContext(SubscriptionKey key, BasicDeliverEventArgs eventArgs)
    {
        Current ??= new Context(key, eventArgs);
    }
    
    /// <summary>
    /// Контекст сообщения
    /// </summary>
    public class Context
    {
        internal Context(SubscriptionKey key, BasicDeliverEventArgs eventArgs)
        {
            SubscriptionKey = key.CheckNotNull();
            EventArgs = eventArgs.CheckNotNull();
        }
        
        /// <summary>
        /// Ключ подписки на событие
        /// </summary>
        public SubscriptionKey SubscriptionKey { get; }
    
        /// <summary>
        /// Аргументы события
        /// </summary>
        public BasicDeliverEventArgs EventArgs { get; }

        /// <summary>
        /// Id корреляции для связи сообщений
        /// </summary>
        public string? CorrelationId => EventArgs.BasicProperties.CorrelationId;

        /// <summary>
        /// Ключ маршрутизации, с которым должно публиковаться ответное сообщение
        /// </summary>
        public string? ReplyTo => EventArgs.BasicProperties.ReplyTo;
    }
}