using Essentials.Utils.Extensions;

namespace Sample.Client.Samples.RpcCallSample;

public class Service : IHostedService
{
    private readonly RpcCallSampleService _service;
    
    public Service(RpcCallSampleService service)
    {
        _service = service.CheckNotNull();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _service.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}