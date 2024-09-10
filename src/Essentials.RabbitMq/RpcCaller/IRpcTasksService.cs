using LanguageExt;

namespace Essentials.RabbitMq.RpcCaller;

/// <summary>
/// Сервис управления задачами Rpc запросов
/// </summary>
internal interface IRpcTasksService
{
    /// <summary>
    /// Регистрирует задачу на осуществление Rpc запроса
    /// </summary>
    /// <param name="correlationId">Id корреляции для связи сообщений</param>
    /// <param name="token">Токен отмены задачи</param>
    /// <returns></returns>
    Try<TaskCompletionSource<IEvent>> RegisterRpcTask(string correlationId, CancellationToken token);
    
    /// <summary>
    /// Проставляет ответ для задачи
    /// </summary>
    /// <param name="correlationId">Id корреляции для связи сообщений</param>
    /// <param name="event">Событие, которое требуется проставить в качестве результата</param>
    /// <returns></returns>
    Try<Unit> SetResult(string correlationId, IEvent @event);
}