using CaseR;

namespace TheStoryVault.Infrastructure.Interceptors;

public class LogIncludedInterceptor<TRequest, TResponse> : IUseCaseInterceptor<TRequest, TResponse>
{
    private readonly ILogger<LogInterceptor> logger;

    public LogIncludedInterceptor(ILogger<LogInterceptor> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> InterceptExecution(IUseCaseInteractor<TRequest, TResponse> useCaseHandler, TRequest request, UseCasePerformDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        this.logger.LogTrace("Entered included {HnadlerName}.",
                useCaseHandler.GetType().Name);

        using IDisposable? scope = this.logger.BeginScope(new Dictionary<string, object>
        {
            ["HandlerName"] = useCaseHandler.GetType().Name,
        });

        return await next(request).ConfigureAwait(false);
    }
}