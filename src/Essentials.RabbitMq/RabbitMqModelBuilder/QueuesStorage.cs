using System.Collections;
using Essentials.RabbitMq.Models;
using Queue = Essentials.RabbitMq.Models.Queue;

namespace Essentials.RabbitMq.RabbitMqModelBuilder;

/// <summary>
/// Хранилище очередей
/// </summary>
internal class QueuesStorage : IEnumerable<(ConnectionName, List<Queue>)>
{
    private static readonly Dictionary<ConnectionName, List<Queue>> _map = [];
    
    /// <summary>
    /// Регистрирует обменник в очереди
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="queue">Очередь</param>
    public static void RegisterQueue(ConnectionName connectionName, Queue queue)
    {
        _map.TryAdd(connectionName, []);
        
        var queues = _map[connectionName];
        var existingQueue = queues.FirstOrDefault(q => q.Name == queue.Name);
        if (existingQueue is null)
        {
            queues.Add(queue);
            return;
        }
        
        queue.AddBindings(existingQueue.Bindings);
        queues[queues.IndexOf(existingQueue)] = queue;
    }
    
    public IEnumerator<(ConnectionName, List<Queue>)> GetEnumerator() =>
        _map
            .Select(pair => (pair.Key, pair.Value))
            .GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}