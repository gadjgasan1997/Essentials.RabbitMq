using RabbitMQ.Client;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Dictionaries;
using Essentials.RabbitMq.RpcCaller.Models;
using Essentials.RabbitMq.RpcCaller.Interception;
using RabbitMQ.Client.Events;

namespace Essentials.RabbitMq.RpcCaller.Configuration.Builders;

/// <inheritdoc cref="IRpcCallConfigurator{TRequest}" />
internal class RpcCallConfigurator<TRequest> : IRpcCallConfigurator<TRequest>
    where TRequest : IRpcCallRequest
{
    private ContentType? _contentType;
    private int? _retryCount;
    private DeliveryMode? _deliveryMode;
    private string? _correlationId;
    private TimeSpan? _timeout;
    private readonly List<Type> _behaviors = [];
    private Action<IBasicProperties>? _configurePropertiesAction;
    private readonly Dictionary<string, object> _headers = [];
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.WithJsonContentType" />
    public IRpcCallConfigurator<TRequest> WithJsonContentType()
    {
        _contentType = ContentType.Json;
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.WithXmlContentType" />
    public IRpcCallConfigurator<TRequest> WithXmlContentType()
    {
        _contentType = ContentType.Xml;
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.SetRetryCount" />
    public IRpcCallConfigurator<TRequest> SetRetryCount(int retryCount)
    {
        _retryCount = retryCount;
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.SetDeliveryMode" />
    public IRpcCallConfigurator<TRequest> SetDeliveryMode(DeliveryMode deliveryMode)
    {
        _deliveryMode = deliveryMode;
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.SetCorrelationId" />
    public IRpcCallConfigurator<TRequest> SetCorrelationId(string correlationId)
    {
        _correlationId = correlationId;
        return this;
    }

    public IRpcCallConfigurator<TRequest> ReceiveCorrelationIdByFunc(Func<BasicDeliverEventArgs, string> func)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.SetTimeout" />
    public IRpcCallConfigurator<TRequest> SetTimeout(TimeSpan timeout)
    {
        _timeout = timeout;
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.AttachBehavior{TBehavior}" />
    public IRpcCallConfigurator<TRequest> AttachBehavior<TBehavior>()
        where TBehavior : IRpcCallBehavior
    {
        _behaviors.Add(typeof(TBehavior));
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.ConfigureProperties" />
    public IRpcCallConfigurator<TRequest> ConfigureProperties(Action<IBasicProperties> configure)
    {
        _configurePropertiesAction = configure;
        return this;
    }
    
    /// <inheritdoc cref="IRpcCallConfigurator{TRequest}.AttachHeader{TValue}" />
    public IRpcCallConfigurator<TRequest> AttachHeader<TValue>(string name, TValue value)
    {
        name.CheckNotNullOrEmpty();
        value.CheckNotNull();
        
        _headers.Add(name, value);
        
        return this;
    }
    
    /// <summary>
    /// Создает опции Rpc запроса
    /// </summary>
    /// <returns>Опции Rpc запроса</returns>
    public RpcCallOptions BuildOptions()
    {
        return new RpcCallOptions(
            _contentType ?? ContentType.Json,
            _retryCount ?? 5,
            _deliveryMode ?? DeliveryMode.Persistent,
            _correlationId ?? Guid.NewGuid().ToString("N"),
            _timeout ?? TimeSpan.FromSeconds(30),
            _behaviors,
            _headers,
            _configurePropertiesAction);
    }
}