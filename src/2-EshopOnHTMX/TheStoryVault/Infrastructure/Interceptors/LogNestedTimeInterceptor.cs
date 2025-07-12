using System.Diagnostics;
using CaseR;

namespace TheStoryVault.Infrastructure.Interceptors;

public class LogInterceptor
{

}


public class LogInterceptor<TRequest, TResponse> : IUseCaseInterceptor<TRequest, TResponse>
{
    private readonly ILogger<LogInterceptor> logger;

    public LogInterceptor(ILogger<LogInterceptor> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> InterceptExecution(IUseCaseInteractor<TRequest, TResponse> useCaseHandler, TRequest request, UseCasePerformDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        this.logger.LogTrace("Entered {HnadlerName}.",
                useCaseHandler.GetType().Name);

        using IDisposable? scope = this.logger.BeginScope(new Dictionary<string, object>
        {
            ["HandlerName"] = useCaseHandler.GetType().Name,
        });

        bool exitedSuccessfully = true;
        try
        {
            return await next(request).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exitedSuccessfully = false;
            this.logger.LogError(ex, "An error occurred in {HandlerName}.", useCaseHandler.GetType().Name);
            throw;
        }
        finally
        {
            if (exitedSuccessfully)
            {
                this.logger.LogInformation("Exited {HnadlerName}.",
                    useCaseHandler.GetType().Name);
            }
        }
    }
}
