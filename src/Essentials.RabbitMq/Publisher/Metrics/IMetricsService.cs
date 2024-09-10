using Essentials.RabbitMq.Publisher.Models;

namespace Essentials.RabbitMq.Publisher.Metrics;

/// <summary>
/// Сервис управления метриками
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Запускает таймер отправки сообщения
    /// </summary>
    /// <param name="publishKey">Ключ публикации события</param>
    /// <returns></returns>
    IDisposable? StartPublishEventTimer(PublishKey publishKey);
    
    /// <summary>
    /// Инкрементирует счетчик количества всех отправленных сообщений
    /// </summary>
    /// <param name="publishKey">Ключ публикации события</param>
    void StartPublishEvent(PublishKey publishKey);
    
    /// <summary>
    /// Инкрементирует счетчик количества успешно отправленных сообщений
    /// </summary>
    /// <param name="publishKey">Ключ публикации события</param>
    void SuccessPublishEvent(PublishKey publishKey);
    
    /// <summary>
    /// Инкрементирует счетчик количества ошибочно отправленных сообщений
    /// </summary>
    /// <param name="publishKey">Ключ публикации события</param>
    void ErrorPublishEvent(PublishKey publishKey);
}