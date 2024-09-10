using System.Collections;
using Essentials.RabbitMq.Models;

namespace Essentials.RabbitMq.RabbitMqModelBuilder;

/// <summary>
/// Хранилище обменников
/// </summary>
internal class ExchangesStorage : IEnumerable<(ConnectionName, List<Exchange>)>
{
    private static readonly Dictionary<ConnectionName, List<Exchange>> _map = [];
    
    /// <summary>
    /// Регистрирует обменник в соединении
    /// </summary>
    /// <param name="connectionName">Название соединения</param>
    /// <param name="exchange">Обменник</param>
    public static void RegisterExchange(ConnectionName connectionName, Exchange exchange)
    {
        _map.TryAdd(connectionName, []);
        
        var exchanges = _map[connectionName];
        var existingExchange = exchanges.FirstOrDefault(e => e.Name == exchange.Name);
        if (existingExchange is null)
        {
            exchanges.Add(exchange);
            return;
        }
        
        exchanges[exchanges.IndexOf(existingExchange)] = exchange;
    }
    
    public IEnumerator<(ConnectionName, List<Exchange>)> GetEnumerator() =>
        _map
            .Select(pair => (pair.Key, pair.Value))
            .GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}