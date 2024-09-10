using LanguageExt;
using RabbitMQ.Client;
using Essentials.Utils.Tasks;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.RpcCaller.Models;
using Essentials.RabbitMq.RpcCaller.Interception;
using Essentials.RabbitMq.RpcCaller.Configuration.Builders;
using Essentials.RabbitMq.ShareInternalServices.Serializer;
using Essentials.RabbitMq.ShareInternalServices.Publisher;

namespace Essentials.RabbitMq.RpcCaller.Implementations;

/// <inheritdoc cref="IRpcCaller" />
internal class RpcCaller : IRpcCaller
{
    private readonly IMessageSerializer _messageSerializer;
    private readonly IConnectionsService _connectionsService;
    private readonly IEnumerable<IRpcCallBehavior> _behaviors;
    private readonly IPublisher _publisher;
    
    public RpcCaller(
        IMessageSerializer messageSerializer,
        IConnectionsService connectionsService,
        IEnumerable<IRpcCallBehavior> behaviors,
        IPublisher publisher)
    {
        _messageSerializer = messageSerializer.CheckNotNull();
        _connectionsService = connectionsService.CheckNotNull();
        _behaviors = behaviors;
        _publisher = publisher.CheckNotNull();
    }
    
    /// <inheritdoc cref="IRpcCaller.CallAsync{TRequest, TResponse}(TRequest, RpcCallKey, CancellationToken?)" />
    public TryAsync<TResponse> CallAsync<TRequest, TResponse>(
        TRequest request,
        RpcCallKey key,
        CancellationToken? token = null)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>
    {
        return async () =>
        {
            // todo static default options
            var options = new RpcCallConfigurator<TRequest>().BuildOptions();
            return await CallAsync<TRequest, TResponse>(request, key, options, token).Try();
        };
    }
    
    /// <inheritdoc cref="IRpcCaller.CallAsync{TRequest, TResponse}(TRequest, RpcCallKey, Action{IRpcCallConfigurator{TRequest}}, CancellationToken?)" />
    public TryAsync<TResponse> CallAsync<TRequest, TResponse>(
        TRequest request,
        RpcCallKey key,
        Action<IRpcCallConfigurator<TRequest>> configure,
        CancellationToken? token = null)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>
    {
        return async () =>
        {
            var configurator = new RpcCallConfigurator<TRequest>();
            configure.Invoke(configurator);
            var options = configurator.BuildOptions();
            
            return await CallAsync<TRequest, TResponse>(request, key, options, token).Try();
        };
    }
    
    private TryAsync<TResponse> CallAsync<TRequest, TResponse>(
        TRequest request,
        RpcCallKey key,
        RpcCallOptions options,
        CancellationToken? token = null)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>
    {
        return async () =>
        {
            token ??= new CancellationTokenSource(options.Timeout).Token;
            
            var source = TasksAwaiter<IEvent>.CreateTaskSource(options.CorrelationId, token);
            return await CallAsync<TRequest, TResponse>(source.Task, request, key, options);
        };
    }

    private async Task<TResponse> CallAsync<TRequest, TResponse>(
        Task<IEvent> task,
        TRequest request,
        RpcCallKey key,
        RpcCallOptions options)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>
    {
        IRpcCallResponse<TRequest>.Request = request;
        
        return await options.Behaviors
            .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
            .OfType<IRpcCallBehavior>()
            .Aggregate(
                (RpcCallDelegate<TRequest, TResponse>) SeedAsync,
                (next, behavior) => async () => await behavior.Handle(next))
            .Invoke();
        
        async Task<TResponse> SeedAsync()
        {
            var channel = _connectionsService.GetOrCreateChannelForPublish(key.ConnectionName);
            var body = _messageSerializer.Serialize(request, options.ContentType);
            var properties = CreateProperties(channel, key.ReplyTo, options);
            
            await _publisher.PublishAsync(
                channel,
                key.ExchangeName,
                key.RoutingKey,
                body,
                properties,
                options.RetryCount);
            
            var @event = await task;
            return (TResponse) @event;
        }
    }
    
    private static IBasicProperties CreateProperties(
        IModel channel,
        RoutingKey replyTo,
        RpcCallOptions rpcCallOptions)
    {
        var properties = channel.CreateBasicProperties();
        
        properties.DeliveryMode = (byte) rpcCallOptions.DeliveryMode;
        properties.Headers = rpcCallOptions.Headers;
        properties.CorrelationId = rpcCallOptions.CorrelationId;
        properties.ReplyTo = replyTo.Value;
        
        rpcCallOptions.ConfigureProperties?.Invoke(properties);
        return properties;
    }
}