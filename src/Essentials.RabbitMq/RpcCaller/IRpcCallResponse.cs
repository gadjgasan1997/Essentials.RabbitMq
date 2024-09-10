using Essentials.Utils.Extensions;

namespace Essentials.RabbitMq.RpcCaller;

/// <summary>
/// Ответ на Rpc запрос
/// </summary>
public interface IRpcCallResponse<out TRequest> : IEvent
    where TRequest : IRpcCallRequest
{
    private static readonly AsyncLocal<TRequest> _current = new();
    
    /// <summary>
    /// Rpc запрос
    /// </summary>
    static TRequest? Request
    {
        get => _current.Value;
        internal set => _current.Value = value.CheckNotNull("Запрос не может быть null здесь!");
    }
}