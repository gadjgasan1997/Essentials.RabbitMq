using Essentials.Utils.Extensions;

namespace Sample.Client.Samples.Sample1;

public class Service : IHostedService
{
    private readonly Sample1Service _service;
    
    public Service(Sample1Service service)
    {
        _service = service.CheckNotNull();
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _service.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}