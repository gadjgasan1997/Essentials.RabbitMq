using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Interception;
using Essentials.RabbitMq.Publisher.Metrics;

namespace Essentials.RabbitMq.Publisher.Interception;

/// <summary>
/// Перехватчик отправки сообщения для сбора метрик
/// </summary>
public class MetricsMessagePublisherBehavior : IMessageBehavior
{
    private readonly IMetricsService _metricsService;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="metricsService">Сервис управления метриками</param>
    public MetricsMessagePublisherBehavior(IMetricsService metricsService)
    {
        _metricsService = metricsService.CheckNotNull();
    }
    
    /// <inheritdoc cref="IMessageBehavior.Handle" />
    public async Task Handle(NextActionDelegate next)
    {
        var context = MessageContext.Current;
        context.CheckNotNull("Context must not be null here!");

        var publishKey = context.PublishKey;
        
        try
        {
            using var _ = _metricsService.StartPublishEventTimer(publishKey);
            _metricsService.StartPublishEvent(publishKey);

            await next();

            _metricsService.SuccessPublishEvent(publishKey);
        }
        catch
        {
            _metricsService.ErrorPublishEvent(publishKey);
            throw;
        }
    }
}