using System.Collections;
using System.Collections.Concurrent;
using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RabbitMqModelBuilder;

/// <summary>
/// Хранилище обменников
/// </summary>
internal class ExchangesStorage : IEnumerable<(ConnectionName, List<Exchange>)>
{
    private static readonly ConcurrentDictionary<ConnectionName, List<Exchange>> _map = [];
    
    /// <summary>
    /// Регистрирует обменник в соединении
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="exchange">Обменник</param>
    public static void RegisterExchange(ConnectionName connectionName, Exchange exchange)
    {
        _map.TryAdd(connectionName, []);
        _map[connectionName].Add(exchange);
    }
    
    public IEnumerator<(ConnectionName, List<Exchange>)> GetEnumerator() =>
        _map
            .Select(pair => (pair.Key, pair.Value))
            .GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}