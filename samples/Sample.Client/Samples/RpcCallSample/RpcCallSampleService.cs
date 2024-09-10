using NLog;
using Essentials.Utils.Extensions;
using Essentials.RabbitMq.RpcCaller;
using Essentials.RabbitMq.RpcCaller.Models;
using Sample.Client.Samples.RpcCallSample.Events;
using static Essentials.RabbitMq.Dictionaries.KnownExchanges;
// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Local

namespace Sample.Client.Samples.RpcCallSample;

public class RpcCallSampleService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    
    private readonly IRpcCaller _rpcCaller;
    
    public RpcCallSampleService(IRpcCaller rpcCaller)
    {
        _rpcCaller = rpcCaller.CheckNotNull();
    }
    
    public async Task RunAsync()
    {
        var request = new TestOutEvent("request");
        var rpcCallKey = new RpcCallKey(
            ConnectionName: "esb",
            ExchangeName: AMQ_DIRECT,
            RoutingKey: "test-rpc-request-queue",
            ReplyTo: $"test-rpc-response-queue-{Environment.GetEnvironmentVariable("APPLICATION_INSTANCE")}");
        
        var result = await _rpcCaller
            .CallAsync<TestOutEvent, TestInEvent>(
                request: request,
                key: rpcCallKey,
                configure: configurator => configurator.SetCorrelationId($"sample-client-{Guid.NewGuid():N}"))
            .Try();
    }
}