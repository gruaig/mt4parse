using Microsoft.Extensions.Hosting;

namespace Mt4LogParser.Engine;

public class Application(LogManager manager) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await manager.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await manager.Stop(cancellationToken);
    }
}