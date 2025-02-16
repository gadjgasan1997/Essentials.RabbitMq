using RabbitMQ.Client;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Publisher.Models;
using Essentials.RabbitMq.Publisher.Interception;

namespace Essentials.RabbitMq.Publisher.Configuration.Builders;

/// <inheritdoc cref="IPublishConfigurator{TEvent}" />
internal class PublishConfigurator<TEvent> : IPublishConfigurator<TEvent>
    where TEvent : IEvent
{
    private ContentType? _contentType;
    private int? _retryCount;
    private DeliveryMode? _deliveryMode;
    private string? _correlationId;
    private string? _replyTo;
    private readonly List<Type> _behaviors = [];
    private Action<IBasicProperties>? _configurePropertiesAction;
    private readonly Dictionary<string, object> _headers = [];
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.WithJsonContentType" />
    public IPublishConfigurator<TEvent> WithJsonContentType()
    {
        _contentType = ContentType.Json;
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.WithXmlContentType" />
    public IPublishConfigurator<TEvent> WithXmlContentType()
    {
        _contentType = ContentType.Xml;
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.SetRetryCount" />
    public IPublishConfigurator<TEvent> SetRetryCount(int retryCount)
    {
        _retryCount = retryCount;
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.SetDeliveryMode" />
    public IPublishConfigurator<TEvent> SetDeliveryMode(DeliveryMode deliveryMode)
    {
        _deliveryMode = deliveryMode;
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.SetCorrelationId" />
    public IPublishConfigurator<TEvent> SetCorrelationId(string correlationId)
    {
        _correlationId = correlationId;
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.SetReplyTo" />
    public IPublishConfigurator<TEvent> SetReplyTo(string replyTo)
    {
        _replyTo = replyTo;
        return this;
    }

    /// <inheritdoc cref="IPublishConfigurator{TEvent}.AttachBehavior{TBehavior}" />
    public IPublishConfigurator<TEvent> AttachBehavior<TBehavior>()
        where TBehavior : IMessageBehavior
    {
        _behaviors.Add(typeof(TBehavior));
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.AttachDefaultMetricsBehavior" />
    public IPublishConfigurator<TEvent> AttachDefaultMetricsBehavior()
    {
        _behaviors.Add(typeof(MetricsMessagePublisherBehavior));
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.AttachDefaultLoggingBehavior" />
    public IPublishConfigurator<TEvent> AttachDefaultLoggingBehavior()
    {
        _behaviors.Add(typeof(LoggingMessagePublisherBehavior));
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.ConfigureProperties" />
    public IPublishConfigurator<TEvent> ConfigureProperties(Action<IBasicProperties> configure)
    {
        _configurePropertiesAction = configure;
        return this;
    }
    
    /// <inheritdoc cref="IPublishConfigurator{TEvent}.AttachHeader{TValue}" />
    public IPublishConfigurator<TEvent> AttachHeader<TValue>(string name, TValue value)
    {
        name.CheckNotNullOrEmpty();
        value.CheckNotNull();
        
        _headers.Add(name, value);
        
        return this;
    }
    
    /// <summary>
    /// Билдит опции публикации события
    /// </summary>
    /// <param name="configure">Действие конфигурации</param>
    /// <returns>Опции</returns>
    public PublishOptions BuildOptions(Action<IPublishConfigurator<TEvent>>? configure = null)
    {
        configure?.Invoke(this);
        
        return new PublishOptions(
            _contentType ?? ContentType.Json,
            _retryCount ?? 5,
            _deliveryMode ?? DeliveryMode.Persistent,
            _correlationId,
            _replyTo,
            _behaviors,
            _headers,
            _configurePropertiesAction);
    }
}