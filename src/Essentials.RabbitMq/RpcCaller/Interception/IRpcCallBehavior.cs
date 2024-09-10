namespace Essentials.RabbitMq.RpcCaller.Interception;

/// <summary>
/// Делегат осуществления Rpc запроса
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public delegate Task<TResponse> RpcCallDelegate<TRequest, TResponse>()
    where TRequest : IRpcCallRequest
    where TResponse : IRpcCallResponse<TRequest>;

/// <summary>
/// Перехватчик Rpc запроса
/// </summary>
public interface IRpcCallBehavior
{
    /// <summary>
    /// Обрабатывает Rpc запрос. Вызывается перед запросом
    /// </summary>
    /// <param name="next">Делегат вызова следующего перехватчика/отправки Rpc запрос</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>Ответ</returns>
    Task<TResponse> Handle<TRequest, TResponse>(RpcCallDelegate<TRequest, TResponse> next)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>;
}