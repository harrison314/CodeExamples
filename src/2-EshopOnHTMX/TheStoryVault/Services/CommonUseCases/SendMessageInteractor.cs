using CaseR;

namespace TheStoryVault.Services.CommonUseCases;

public class SendMessageInteractor : IUseCaseInteractor<string, Unit>
{
    private readonly ILogger<SendMessageInteractor> logger;

    public SendMessageInteractor(ILogger<SendMessageInteractor> logger)
    {
        this.logger = logger;
    }

    public async Task<Unit> Execute(string request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Message sent: {Message}", request);
        await Task.Delay(20, cancellationToken);

        return Unit.Value;
    }
}
