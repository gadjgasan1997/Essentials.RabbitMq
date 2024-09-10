using LanguageExt;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Subscriber.Models;
using Essentials.RabbitMq.Subscriber.Configuration;
using Essentials.RabbitMq.Subscriber.Implementations;

namespace Essentials.RabbitMq.Subscriber;

/// <summary>
/// Сервис для автоматической подписки на события
/// </summary>
internal class AutoSubscribeHostedService : IHostedService
{
    private readonly InternalEventsSubscriber _eventsSubscriber;
    private readonly ILogger<AutoSubscribeHostedService> _logger;
    
    public AutoSubscribeHostedService(
        InternalEventsSubscriber eventsSubscriber,
        ILogger<AutoSubscribeHostedService> logger)
    {
        _eventsSubscriber = eventsSubscriber.CheckNotNull();
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SubscribeForEvents();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Во время автоматической подписки на события произошло исключение");
            
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    
    private async Task SubscribeForEvents()
    {
        var method = typeof(InternalEventsSubscriber)
            .GetMethod(
                nameof(InternalEventsSubscriber.SubscribeWithExistingOptionsAsync),
                bindingAttr: BindingFlags.Instance | BindingFlags.Public);

        method.CheckNotNull(
            $"Не найден метод с названием '{nameof(InternalEventsSubscriber.SubscribeWithExistingOptionsAsync)}' " +
            "для автоматической подписки на события");

        foreach (var (eventType, key, options) in new Storage())
            await SubscribeForEvent(method, eventType, key, options);
    }
    
    private async Task SubscribeForEvent(
        MethodInfo method,
        Type eventType,
        SubscriptionKey key,
        SubscriptionOptions options)
    {
        try
        {
            var genericMethod = method.MakeGenericMethod(eventType);
            
            var result = genericMethod.Invoke(_eventsSubscriber, [key, options]);
            if (result is not TryAsync<Unit> @try)
            {
                throw new InvalidOperationException(
                    $"Результат вызова метода '{nameof(InternalEventsSubscriber.SubscribeWithExistingOptionsAsync)}' " +
                    $"не соответствует типу '{nameof(TryAsync<Unit>)}'");
            }
            
            _ = await @try.IfFailThrow();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Во время автоматической подписки на событие с типом '{type}' произошло исключение",
                eventType.FullName);
            
            throw;
        }
    }
}