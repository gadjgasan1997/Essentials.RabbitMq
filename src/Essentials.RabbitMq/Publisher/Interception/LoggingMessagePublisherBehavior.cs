using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Essentials.Utils.Extensions;
using Essentials.Logging.Extensions;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Publisher.Models;
using static System.DateTime;
using static System.Environment;
using static Essentials.Utils.Dictionaries.KnownDatesFormats;
using static Essentials.Serialization.Helpers.JsonHelpers;

namespace Essentials.RabbitMq.Publisher.Interception;

/// <summary>
/// Перехватчик отправки сообщения для сбора логов
/// </summary>
public class LoggingMessagePublisherBehavior : IMessageBehavior
{
    private readonly ILoggerFactory _factory;
    private readonly ConcurrentDictionary<PublishKey, ILogger> _loggersMap = new();
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="factory">Фабрика получения логгера</param>
    public LoggingMessagePublisherBehavior(ILoggerFactory factory)
    {
        _factory = factory;
    }
    
    /// <inheritdoc cref="IMessageBehavior.Handle" />
    public async Task Handle(NextActionDelegate next)
    {
        var context = MessageContext.Current;
        context.CheckNotNull("Context must not be null here!");
        
        var clock = new Stopwatch();
        clock.Start();
        
        var publishKey = context.PublishKey;
        var logger = GetLogger(context, publishKey);
        
        using var _ = logger.BeginScope(
            new Dictionary<string, object?>
            {
                ["rabbit_message_id"] = Guid.NewGuid()
            });
        
        try
        { 
            logger
                .LogIfLevelIsTrace(() =>
                {
                    logger.LogTrace(
                        "Начинается публикация сообщения с ключом '{routingKey}' в обменник '{exchange}'" +
                        "{newLine}Текущая дата/время: '{date}'" +
                        "{newLine}Содержимое: '{content}'",
                        publishKey.RoutingKey.Value, publishKey.ExchangeName.Value,
                        NewLine, Now.ToString(LogDateLongFormat),
                        NewLine, Serialize(context.Content));
                })
                .LogIfLevelIsDebugOrHigher(() =>
                {
                    logger.LogInformation(
                        "Начинается публикация сообщения с ключом '{routingKey}' в обменник '{exchange}'" +
                        "{newLine}Текущая дата/время: '{date}'",
                        publishKey.RoutingKey.Value, publishKey.ExchangeName.Value,
                        NewLine, Now.ToString(LogDateLongFormat));
                });

            await next();
            
            clock.Stop();

            logger.LogIfLevelIsInfoOrLow(() =>
            {
                logger.LogInformation(
                    "Сообщение с ключом '{routingKey}' было успешно опубликовано в обменник '{exchange}'" +
                    "{newLine}Текущая дата/время: '{date}'. Затраченное время: '{elapsed}' мс",
                    publishKey.RoutingKey.Value, publishKey.ExchangeName.Value,
                    NewLine, Now.ToString(LogDateLongFormat), clock.ElapsedMilliseconds);
            });
        }
        catch (Exception exception)
        {
            clock.Stop();
            
            logger.LogError(
                exception,
                "Во время публикации сообщения с ключом '{routingKey}' в обменник '{exchange}' произошло исключение" +
                "{newLine}Текущая дата/время: '{date}'. Затраченное время: '{elapsed}' мс" +
                "{newLine}Содержимое: '{content}'",
                publishKey.RoutingKey.Value, publishKey.ExchangeName.Value,
                NewLine, Now.ToString(LogDateLongFormat), clock.ElapsedMilliseconds,
                NewLine, Serialize(context.Content));
            
            throw;
        }
    }

    /// <summary>
    /// Возвращает логгер
    /// </summary>
    /// <param name="context">Контекст сообщения</param>
    /// <param name="publishKey">Ключ публикации события</param>
    /// <returns>Логгер</returns>
    private ILogger GetLogger(MessageContext.Context context, PublishKey publishKey)
    {
        return _loggersMap.GetOrAdd(
            publishKey,
            _ =>
            {
                var loggerName = GetLoggerName(context);
                return _factory.CreateLogger(loggerName);
            });
    }
    
    /// <summary>
    /// Возвращает название логгера
    /// </summary>
    /// <param name="context">Контекст сообщения</param>
    /// <returns>Название логгера</returns>
    protected virtual string GetLoggerName(MessageContext.Context context)
    {
        var (connectionName, exchangeName, routingKey) = context.PublishKey;
        
        return $"Essentials.RabbitMq.PublishMessageBehavior." +
               $"{connectionName.Value}." +
               $"{(routingKey.IsEmpty ? exchangeName.Value : routingKey.Value)}";
    }
}