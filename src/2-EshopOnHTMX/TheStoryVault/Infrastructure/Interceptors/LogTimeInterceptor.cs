using Azure.Core;
using System.Diagnostics;
using CaseR;

namespace TheStoryVault.Infrastructure.Interceptors;

public class LogTimeInterceptor
{

}
public class LogTimeInterceptor<TRequest, TResponse> : IUseCaseInterceptor<TRequest, TResponse>
{
    private readonly ILogger<LogTimeInterceptor> logger;

    public LogTimeInterceptor(ILogger<LogTimeInterceptor> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> InterceptExecution(IUseCaseInteractor<TRequest, TResponse> useCaseHandler, TRequest request, UseCasePerformDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        long timestamp = Stopwatch.GetTimestamp();
        try
        {
            return await next(request).ConfigureAwait(false);
        }
        finally
        {
            TimeSpan elapedTime = Stopwatch.GetElapsedTime(timestamp);

            this.logger.LogInformation("Elaped {ElapsedTime}ms in {HnadlerName}.",
                elapedTime.TotalMilliseconds,
                useCaseHandler.GetType().Name);
        }
    }
}
