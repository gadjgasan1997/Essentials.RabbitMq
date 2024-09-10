using App.Metrics;
using App.Metrics.Counter;
using System.Collections.Concurrent;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.Publisher.Models;
using static Essentials.RabbitMq.Publisher.Metrics.MetricsRegistry;

namespace Essentials.RabbitMq.Publisher.Metrics.Implementations;

/// <inheritdoc cref="IMetricsService" />
internal class MetricsService : IMetricsService
{
    private readonly IMetrics _metrics;
    
    private static readonly ConcurrentDictionary<PublishKey, MetricTags> _publishTags = new();
    
    public MetricsService(IMetrics metrics)
    {
        _metrics = metrics.CheckNotNull();
    }
    
    /// <inheritdoc cref="IMetricsService.StartPublishEventTimer" />
    public IDisposable StartPublishEventTimer(PublishKey publishKey)
    {
        var tags = _publishTags.GetOrAdd(publishKey, _ => GetMetricPublishTags(publishKey));
        return _metrics.Measure.Timer.Time(OutEventsPublishTimer, tags);
    }

    /// <inheritdoc cref="IMetricsService.StartPublishEvent" />
    public void StartPublishEvent(PublishKey publishKey) =>
        IncrementPublishCounter(publishKey, OutEventsCounterOptions);

    /// <inheritdoc cref="IMetricsService.SuccessPublishEvent" />
    public void SuccessPublishEvent(PublishKey publishKey) =>
        IncrementPublishCounter(publishKey, ValidOutEventsCounterOptions);

    /// <inheritdoc cref="IMetricsService.ErrorPublishEvent" />
    public void ErrorPublishEvent(PublishKey publishKey) =>
        IncrementPublishCounter(publishKey, InvalidOutEventsCounterOptions);
    
    /// <summary>
    /// Инкрементирует счетчик количества отправленных сообщений
    /// </summary>
    /// <param name="publishKey">Ключ публикации события</param>
    /// <param name="counter">Счетчик</param>
    private void IncrementPublishCounter(PublishKey publishKey, CounterOptions counter)
    {
        var tags = _publishTags.GetOrAdd(publishKey, _ => GetMetricPublishTags(publishKey));
        _metrics.Measure.Counter.Increment(counter, tags);
    }
    
    /// <summary>
    /// Возвращает теги метрик для публикации сообщения
    /// </summary>
    /// <param name="publishKey">Ключ публикации события</param>
    /// <returns>Теги</returns>
    private static MetricTags GetMetricPublishTags(PublishKey publishKey) =>
        new(
            keys: ["exchange_name", "routing_key"],
            values:
            [
                publishKey.ExchangeName.Value,
                publishKey.RoutingKey.IsEmpty ? "undefined_key" : publishKey.RoutingKey.Value
            ]);
}