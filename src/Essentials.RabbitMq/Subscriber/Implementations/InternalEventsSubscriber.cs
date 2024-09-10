using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using Essentials.Utils.Extensions;
using Essentials.Logging.Extensions;
using Essentials.Functional.Extensions;
using Essentials.RabbitMq.Models;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Extensions;
using Essentials.RabbitMq.Subscriber.Configuration.Builders;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static System.Environment;
// ReSharper disable ConvertToLambdaExpression

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <summary>
/// Подписчик на события
/// </summary>
internal class InternalEventsSubscriber
{
    private readonly IConnectionsService _connectionsService;
    private readonly IConfigurationManager _configurationManager;
    private readonly IEventsHandlerService _eventsHandlerService;
    private readonly ILogger _logger;
    
    public InternalEventsSubscriber(
        IConnectionsService connectionsService,
        IConfigurationManager configurationManager,
        IEventsHandlerService eventsHandlerService,
        ILoggerFactory loggerFactory)
    {
        _connectionsService = connectionsService.CheckNotNull();
        _configurationManager = configurationManager.CheckNotNull();
        _eventsHandlerService = eventsHandlerService.CheckNotNull();
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.Subscriber");
    }
    
    /// <summary>
    /// Подписывается на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="configure">Действие конфигурации опций подписки</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public Try<Unit> Subscribe<TEvent>(
        SubscriptionKey key,
        Action<ISubscriptionConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        return CreateEventSubscriptionOptions(key, configure)
            .Bind(SaveEventSubscriptionOptions<TEvent>)
            .Bind(RegisterEventHandler<TEvent>)
            .Bind(ConsumeEvent);
    }
    
    /// <summary>
    /// Подписывается на событие с известными опциями подписки
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public Try<Unit> SubscribeWithExistingOptions<TEvent>(
        InternalSubscriptionKey key,
        SubscriptionOptions options)
        where TEvent : IEvent
    {
        return SaveEventSubscriptionOptions<TEvent>((key, options))
            .Bind(RegisterEventHandler<TEvent>)
            .Bind(ConsumeEvent);
    }

    private static Try<(InternalSubscriptionKey, SubscriptionOptions)> CreateEventSubscriptionOptions<TEvent>(
        SubscriptionKey key,
        Action<ISubscriptionConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        return Prelude.Try(() =>
        {
            var connectionName = new ConnectionName(key.ConnectionName);
            var queueName = new QueueName(key.QueueName);
            var subscriptionKey = new InternalSubscriptionKey(connectionName, queueName);
            var subscriptionOptions = GetOptions(configure);

            return (subscriptionKey, subscriptionOptions);
        });
    }

    private Try<(InternalSubscriptionKey, SubscriptionOptions)> SaveEventSubscriptionOptions<TEvent>(
        (InternalSubscriptionKey, SubscriptionOptions) tuple)
        where TEvent : IEvent
    {
        return _configurationManager
            .SaveSubscriptionOptions<TEvent>(tuple.Item1, tuple.Item2)
            .Map(_ => tuple);
    }

    private Try<(InternalSubscriptionKey, SubscriptionOptions)> RegisterEventHandler<TEvent>(
        (InternalSubscriptionKey, SubscriptionOptions) tuple)
        where TEvent : IEvent
    {
        return () =>
        {
            _eventsHandlerService.RegisterEventHandler<TEvent>(tuple.Item1, tuple.Item2);
            return tuple;
        };
    }

    private Try<Unit> ConsumeEvent((InternalSubscriptionKey, SubscriptionOptions) tuple)
    {
        return () =>
        {
            var channel = _connectionsService.GetOrCreateChannel(tuple.Item1, tuple.Item2);
            var consumer = _connectionsService.CreateConsumer(channel, tuple.Item1);
            consumer.Received += ConsumerReceived;
            
            return Unit.Default;
        };
    }

    private static SubscriptionOptions GetOptions<TEvent>(Action<ISubscriptionConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        var configurator = new SubscriptionConfigurator<TEvent>();
        configure?.Invoke(configurator);
        return configurator.BuildSubscriptionOptions();
    }

    private async Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        if (sender is not AsyncEventingBasicConsumer consumer)
        {
            _logger.LogError(
                "Слушатель не является типом '{typeName}'" +
                "{newLine}Аргументы: '{args}'",
                typeof(AsyncEventingBasicConsumer).FullName,
                NewLine, Serialize(eventArgs));
            
            return;
        }
        
        _ = await GetSubscriptionKey(consumer)
            .Bind(GetSubscriptionOptions)
            .ToValidation(Error.New)
            .BindAsync(tuple => HandleEventAsync(eventArgs, tuple.Item1, tuple.Item2))
            .MatchAsync(
                Succ: _ => OnSuccessProcessEvent(consumer, eventArgs),
                Fail: errors => OnFailProcessEvent(consumer, eventArgs, errors));
    }

    private Try<InternalSubscriptionKey> GetSubscriptionKey(AsyncEventingBasicConsumer consumer)
    {
        return () =>
        {
            return _connectionsService
                .GetSubscriptionKey(consumer.ConsumerTags)
                .IfNone(() =>
                {
                    throw new KeyNotFoundException(
                        $"Не найден ключ подписки по тегам: '{consumer.GetConsumerTagsString()}'");
                });
        };
    }

    private Try<Tuple<InternalSubscriptionKey, SubscriptionOptions>> GetSubscriptionOptions(
        InternalSubscriptionKey key)
    {
        return _configurationManager
            .GetSubscriptionOptionsByKey(key)
            .Map(options => new Tuple<InternalSubscriptionKey, SubscriptionOptions>(key, options));
    }

    private async Task<Validation<Error, Unit>> HandleEventAsync(
        BasicDeliverEventArgs eventArgs,
        InternalSubscriptionKey key,
        SubscriptionOptions options)
    {
        return await Prelude
            .TryAsync(async () =>
            {
                MessageContext.CreateContext(key, eventArgs);
                
                await _eventsHandlerService.HandleEvent(key, options, eventArgs);
                
                return Unit.Default;
            })
            .Match(
                Succ: Prelude.Success<Error, Unit>,
                Fail: exception => Error.New(exception));
    }

    private Unit OnSuccessProcessEvent(
        AsyncEventingBasicConsumer consumer,
        BasicDeliverEventArgs eventArgs)
    {
        _logger.LogIfLevelIsDebugOrLow(() =>
        {
            _logger.LogDebug(
                "Сообщение с тегами '{tags}' было успешно обработано",
                consumer.GetConsumerTagsString());
        });
        
        consumer.Model.BasicAck(eventArgs.DeliveryTag, false);
        return Unit.Default;
    }

    private Unit OnFailProcessEvent(
        AsyncEventingBasicConsumer consumer,
        BasicDeliverEventArgs eventArgs,
        Seq<Error> errors)
    {
        _logger.LogError(
            errors.ToAggregateException(),
            "Ошибка обработки сообщения с тегами '{tags}'",
            consumer.GetConsumerTagsString());
        
        consumer.Model.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
        return Unit.Default;
    }
}