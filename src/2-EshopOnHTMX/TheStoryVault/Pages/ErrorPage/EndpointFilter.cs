
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace TheStoryVault.Pages.ErrorPage;

public class EndpointFilter : IEndpointFilter
{
    public EndpointFilter()
    {
        
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            object? result = await next(context);
            return result;
        }
        catch (Exception ex)
        {
            ILogger logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<EndpointFilter>>();
            logger.LogError(ex, "An error occurred while processing the request.");

            string errorMessage = ex.Message;

            if (context.HttpContext.IsHtmxRequest())
            {
                context.HttpContext.SendHtmxRetarget("main");
                context.HttpContext.SendHtmxReswap("innerHTML");
                return new RazorComponentResult<TheStoryVault.Pages.ErrorPage.MainError>(new Dictionary<string, object?>()
                {
                    { nameof(MainError.ErrorMessage), errorMessage },
                })
                { 
                    StatusCode = 500
                };
            }
            else
            {
                return new RazorComponentResult<TheStoryVault.Pages.ErrorPage.MainView>(new Dictionary<string, object?>()
                {
                    { nameof(MainError.ErrorMessage), errorMessage },
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
