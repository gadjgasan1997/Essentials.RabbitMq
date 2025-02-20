﻿using LanguageExt;
using RabbitMQ.Client.Events;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Logging;
using Essentials.Utils.Extensions;
using Essentials.Logging.Extensions;
using Essentials.Functional.Extensions;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Extensions;
using Essentials.RabbitMq.Subscriber.Configuration.Builders;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static LanguageExt.Prelude;
using static System.Environment;
// ReSharper disable ConvertToLambdaExpression

namespace Essentials.RabbitMq.Subscriber.Implementations;

/// <summary>
/// Подписчик на события
/// </summary>
internal class InternalEventsSubscriber
{
    private readonly IConnectionsService _connectionsService;
    private readonly IEventsHandlerService _eventsHandlerService;
    private readonly IFeatureManager _featureManager;
    private readonly ILogger _logger;
    
    public InternalEventsSubscriber(
        IConnectionsService connectionsService,
        IEventsHandlerService eventsHandlerService,
        IFeatureManager featureManager,
        ILoggerFactory loggerFactory)
    {
        _connectionsService = connectionsService.CheckNotNull();
        _eventsHandlerService = eventsHandlerService.CheckNotNull();
        _featureManager = featureManager.CheckNotNull();
        _logger = loggerFactory.CreateLogger("Essentials.RabbitMq.Subscriber");
    }
    
    /// <summary>
    /// Подписывается на событие
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="configure">Действие конфигурации опций подписки</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public TryAsync<Unit> SubscribeAsync<TEvent>(
        SubscriptionKey key,
        Action<ISubscriptionConfigurator<TEvent>>? configure = null)
        where TEvent : IEvent
    {
        return Try(() => new SubscriptionConfigurator<TEvent>().BuildOptions(configure))
            .ToAsync()
            .Bind(options => SubscribeWithExistingOptionsAsync<TEvent>(key, options));
    }
    
    /// <summary>
    /// Подписывается на событие с известными опциями подписки
    /// </summary>
    /// <param name="key">Ключ подписки на событие</param>
    /// <param name="options">Опции подписки на событие</param>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <returns></returns>
    public TryAsync<Unit> SubscribeWithExistingOptionsAsync<TEvent>(
        SubscriptionKey key,
        SubscriptionOptions options)
        where TEvent : IEvent
    {
        return CheckFeatureIsEnabledAsync(key, options)
            .Do(_ => _eventsHandlerService.RegisterEventHandler<TEvent>(key, options))
            .Do(_ => _connectionsService.Consume(key, options, ConsumerReceived))
            .ToTry(() => Unit.Default);
    }
    
    private TryOptionAsync<Unit> CheckFeatureIsEnabledAsync(SubscriptionKey key, SubscriptionOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.FeatureFlag))
            return TryOptionAsync(Unit.Default);
        
        return TryOptionAsync(async () =>
        {
            if (await _featureManager.IsEnabledAsync(options.FeatureFlag))
                return Unit.Default;
            
            OnFeatureIsDisabled(options.FeatureFlag, key);
            return Option<Unit>.None;
        });
    }
    
    private void OnFeatureIsDisabled(string featureFlag, SubscriptionKey key)
    {
        _logger.LogIfLevelIsWarnOrLow(() =>
        {
            _logger.LogWarning(
                "Сообщения из очереди '{queueName}' в соединении '{connectionName}' " +
                "по флагу '{featureFlag}' вычитываться не будут. " +
                "Включите флаг если требуется обрабатывать данные сообщения",
                key.QueueName.Value, key.ConnectionName.Value, featureFlag);
        });
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
        
        _ = await TryOptionAsync(async () =>
            {
                var key = SubscriptionKey.Create(eventArgs);
                
                MessageContext.CreateContext(key, eventArgs);
                
                return await _eventsHandlerService.HandleEventAsync(key, eventArgs).IfFailThrowAsync();
            })
            .Match(
                Some: _ => OnSuccessProcessEvent(consumer, eventArgs),
                None: () => OnUnknownEvent(consumer, eventArgs),
                Fail: exception => OnFailProcessEvent(consumer, eventArgs, exception));

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
    
    private Unit OnUnknownEvent(
        AsyncEventingBasicConsumer consumer,
        BasicDeliverEventArgs eventArgs)
    {
        _logger.LogIfLevelIsDebugOrLow(() =>
        {
            _logger.LogDebug(
                "Сообщение с тегами '{tags}' не было обработано, так как для него не задан обработчик",
                consumer.GetConsumerTagsString());
        });
        
        consumer.Model.BasicReject(eventArgs.DeliveryTag, false);
        return Unit.Default;
    }
    
    private Unit OnFailProcessEvent(
        AsyncEventingBasicConsumer consumer,
        BasicDeliverEventArgs eventArgs,
        Exception exception)
    {
        _logger.LogError(
            exception,
            "Ошибка обработки сообщения с тегами '{tags}'",
            consumer.GetConsumerTagsString());
        
        consumer.Model.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
        return Unit.Default;
    }
}