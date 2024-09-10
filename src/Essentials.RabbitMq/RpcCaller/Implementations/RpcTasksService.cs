using LanguageExt;
using Essentials.Utils.Extensions;
using System.Collections.Concurrent;

namespace Essentials.RabbitMq.RpcCaller.Implementations;

/// <inheritdoc cref="IRpcTasksService" />
internal class RpcTasksService : IRpcTasksService
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IEvent>> _tasks = [];
    
    /// <inheritdoc cref="IRpcTasksService.RegisterRpcTask" />
    public Try<TaskCompletionSource<IEvent>> RegisterRpcTask(string correlationId, CancellationToken token)
    {
        return () =>
        {
            correlationId.CheckNotNullOrEmpty("Id корреляции для связи сообщений не может быть пустым");
            
            var taskCompletionSource = new TaskCompletionSource<IEvent>(TaskCreationOptions.None);
            
            token.Register(() =>
            {
                if (!_tasks.TryRemove(correlationId, out var existingTask))
                {
                    throw new InvalidOperationException(
                        $"Не удалось удалить задачу по correlationId: '{correlationId}'");
                }

                var exception = new TimeoutException($"Таймаут получения ответа по correlationId: '{correlationId}'");
                if (!existingTask.TrySetException(exception))
                    existingTask.SetCanceled();
            });
            
            return _tasks.AddOrUpdate(
                correlationId,
                addValue: taskCompletionSource,
                updateValueFactory: (_, source) => source);
        };
    }
    
    /// <inheritdoc cref="IRpcTasksService.SetResult" />
    public Try<Unit> SetResult(string correlationId, IEvent @event)
    {
        return () =>
        {
            correlationId.CheckNotNullOrEmpty("Id корреляции для связи сообщений не может быть пустым");
            @event.CheckNotNull("Событие, которое требуется проставить в качестве результата, не может быть пустым");
            
            if (!_tasks.TryGetValue(correlationId, out var taskCompletionSource))
            {
                throw new KeyNotFoundException(
                    $"Не удалось найти задачу с CorrelationId, равным '{correlationId}' " +
                    "для проставления результата");
            }
            
            if (!taskCompletionSource.TrySetResult(@event))
            {
                throw new KeyNotFoundException(
                    $"Не удалось проставить результат для задачи с CorrelationId, равным '{correlationId}'");
            }
            
            return Unit.Default;
        };
    }
}