using LanguageExt;
using Essentials.RabbitMq.RpcCaller.Models;
using Essentials.RabbitMq.RpcCaller.Configuration.Builders;

namespace Essentials.RabbitMq.RpcCaller;

/// <summary>
/// Интерфейс для отправки Rpc запросов
/// </summary>
public interface IRpcCaller
{
    /// <summary>
    /// Отправляет Rpc запрос
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="key">Ключ Rpc запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns></returns>
    TryAsync<TResponse> CallAsync<TRequest, TResponse>(
        TRequest request,
        RpcCallKey key,
        CancellationToken? token = null)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>;
    
    /// <summary>
    /// Отправляет Rpc запрос
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="key">Ключ Rpc запроса</param>
    /// <param name="configure">Действие конфигурации</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns></returns>
    TryAsync<TResponse> CallAsync<TRequest, TResponse>(
        TRequest request,
        RpcCallKey key,
        Action<IRpcCallConfigurator<TRequest>> configure,
        CancellationToken? token = null)
        where TRequest : IRpcCallRequest
        where TResponse : IRpcCallResponse<TRequest>;
}