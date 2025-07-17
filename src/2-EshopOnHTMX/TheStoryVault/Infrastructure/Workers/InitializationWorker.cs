using TheStoryVault.Infrastructure.Migration;
using TheStoryVault.Services.Data;

namespace TheStoryVault.Infrastructure.Workers;

public class InitializationWorker : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<InitializationWorker> logger;

    public InitializationWorker(IServiceProvider serviceProvider, ILogger<InitializationWorker> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (Environment.GetCommandLineArgs().Contains("--migrate"))
        {
            this.logger.LogInformation("Starting migration...");
            try
            {
                await Migrator.Migrate(this.serviceProvider, null);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred during migration.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

