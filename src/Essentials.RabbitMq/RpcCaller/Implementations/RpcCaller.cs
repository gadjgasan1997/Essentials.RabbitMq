using Polly;
using LanguageExt;
using System.Net.Sockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Serialization;
using Essentials.RabbitMq.RpcCaller.Models;
using Essentials.RabbitMq.RpcCaller.Interception;
using Essentials.RabbitMq.RpcCaller.Configuration.Builders;
using static System.Environment;
// ReSharper disable ConvertToLambdaExpression

namespace Essentials.RabbitMq.RpcCaller.Implementations;

/// <inheritdoc cref="IRpcCaller" />
internal class RpcCaller : IRpcCaller
{
    private readonly IMessageSerializer _messageSerializer;
    private readonly IConnectionsService _connectionsService;
    private readonly IRpcTasksService _rpcTasksService;
    private readonly IEnumerable<IRpcCallBehavior> _behaviors;
    private readonly ILogger _logger;
    
    public RpcCaller(
        IMessageSerializer messageSerializer,
        IConnectionsService connectionsService,
        IRpcTasksService rpcTasksService,
        IEnumerable<IRpcCallBehavior> behaviors,
        ILoggerFactory loggerFactory)
    {
        _messageSerializer = messageSerializer.CheckNotNull();
        _connectionsService = connectionsService.CheckNotNull();
        _rpcTasksService = rpcTasksService.CheckNotNull();
        _behaviors = behaviors;
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.RpcCaller");
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
            
            var connectionNameModel = ConnectionName.Create(key.ConnectionName);
            var exchangeNameModel = ExchangeName.Create(key.ExchangeName);
            var routingKeyModel = RoutingKey.Create(key.RoutingKey);
            var replyToModel = RoutingKey.Create(key.ReplyTo);
            
            return await _rpcTasksService
                .RegisterRpcTask(options.CorrelationId, token.Value)
                .MapAsync(async taskCompletionSource =>
                {
                    return await CallAsync<TRequest, TResponse>(
                        taskCompletionSource.Task,
                        request,
                        connectionNameModel,
                        exchangeNameModel,
                        routingKeyModel,
                        replyToModel,
                        options);
                })
                .Try();
        };
    }

    private async Task<TResponse> CallAsync<TRequest, TResponse>(
        Task<IEvent> task,
        TRequest request,
        ConnectionName connectionName,
        ExchangeName exchangeName,
        RoutingKey routingKey,
        RoutingKey replyTo,
        RpcCallOptions rpcCallOptions)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>
    {
        IRpcCallResponse<TRequest>.Request = request;
        
        return await rpcCallOptions.Behaviors
            .Select(type => _behaviors.FirstOrDefault(behavior => behavior.GetType() == type))
            .OfType<IRpcCallBehavior>()
            .Aggregate(
                (RpcCallDelegate<TRequest, TResponse>) SeedAsync,
                (next, behavior) => async () => await behavior.Handle(next))
            .Invoke();
        
        async Task<TResponse> SeedAsync()
        {
            PublishWithPolicy(
                request,
                connectionName,
                exchangeName,
                routingKey,
                replyTo,
                rpcCallOptions);
            
            var @event = await task;
            return (TResponse) @event;
        }
    }
    
    private void PublishWithPolicy<TRequest>(
        TRequest request,
        ConnectionName connectionName,
        ExchangeName exchangeName,
        RoutingKey routingKey,
        RoutingKey replyTo,
        RpcCallOptions rpcCallOptions)
        where TRequest : IRpcCallRequest
    {
        var channel = _connectionsService.GetOrCreateChannelForPublish(connectionName);
        var properties = CreateProperties(channel, replyTo, rpcCallOptions);
        var body = _messageSerializer.Serialize(request, rpcCallOptions.ContentType);
        
        Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<Exception>()
            .WaitAndRetry(
                retryCount: rpcCallOptions.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, time) =>
                {
                    _logger.LogError(
                        exception,
                        "Не удалось опубликовать сообщение по происшествии {seconds} секунд." +
                        "{newLine}Название обменника: '{exchange}'." +
                        "{newLine}Ключ маршрутизации: '{publishKey.RoutingKey?.Key}'.",
                        time.TotalSeconds,
                        NewLine, exchangeName.Value,
                        NewLine, routingKey.Value);
                })
            .Execute(() =>
            {
                channel.BasicPublish(
                    exchange: exchangeName.Value,
                    routingKey: routingKey.Value,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
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