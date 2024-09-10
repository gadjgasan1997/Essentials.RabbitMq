using Essentials.Utils.Extensions;

namespace Sample.Server.Samples.Sample2;

public class Service : IHostedService
{
    private readonly Sample2Service _service;
    
    public Service(Sample2Service service)
    {
        _service = service.CheckNotNull();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _service.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}