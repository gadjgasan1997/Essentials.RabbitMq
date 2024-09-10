using Essentials.Utils.Extensions;

namespace Sample.Server.Samples.OneQueueManyRoutingKeysSample;

public class Service : IHostedService
{
    private readonly OneQueueManyRoutingKeysSampleService _service;

    public Service(OneQueueManyRoutingKeysSampleService service)
    {
        _service = service.CheckNotNull();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _service.RunAsync();
    }
    
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}