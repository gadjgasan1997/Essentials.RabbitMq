using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher.Metrics.Implementations;

/// <inheritdoc cref="IMetricsService" />
internal class MockMetricsService : IMetricsService
{
    /// <inheritdoc cref="IMetricsService.StartPublishEventTimer" />
    public IDisposable? StartPublishEventTimer(PublishKey publishKey) => null;

    /// <inheritdoc cref="IMetricsService.StartPublishEvent" />
    public void StartPublishEvent(PublishKey publishKey) { }

    /// <inheritdoc cref="IMetricsService.SuccessPublishEvent" />
    public void SuccessPublishEvent(PublishKey publishKey) { }

    /// <inheritdoc cref="IMetricsService.ErrorPublishEvent" />
    public void ErrorPublishEvent(PublishKey publishKey) { }
}