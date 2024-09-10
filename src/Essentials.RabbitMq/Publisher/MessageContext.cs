using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher;

/// <summary>
/// Контекст публикуемого сообщения
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
    /// <param name="publishKey">Ключ публикации события</param>
    /// <param name="content">Содержимое</param>
    internal static void CreateContext(PublishKey publishKey, object content)
    {
        Current ??= new Context(publishKey, content);
    }
    
    /// <summary>
    /// Контекст сообщения
    /// </summary>
    public class Context
    {
        internal Context(PublishKey publishKey, object content)
        {
            PublishKey = publishKey.CheckNotNull();
            Content = content.CheckNotNull();
        }
        
        /// <summary>
        /// Ключ публикации события
        /// </summary>
        public PublishKey PublishKey { get; }
    
        /// <summary>
        /// Содержимое
        /// </summary>
        public object Content { get; }
    }
}