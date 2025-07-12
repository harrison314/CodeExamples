using Microsoft.AspNetCore.Http.HttpResults;

namespace TheStoryVault.Pages.ErrorPage;

public static class AppErrorExtensions
{
    public static void UseExceptionCustomPage(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appConfigure =>
        {
            appConfigure.Run(async context =>
            {
                Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature? errorFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

                ILogger logger = context.RequestServices.GetRequiredService<ILogger<EndpointFilter>>();
                if (errorFeature == null)
                {
                    logger.LogError("An error occurred, but no exception feature was found.");
                    context.Response.StatusCode = 500;
                    return;
                }

                logger.LogError(errorFeature.Error, "An error occurred while processing the request.");

                string errorMessage = errorFeature.Error.Message;

                IResult result;
                if (context.IsHtmxRequest())
                {
                    context.SendHtmxRetarget("main");
                    context.SendHtmxReswap("innerHTML");
                    result = new RazorComponentResult<TheStoryVault.Pages.ErrorPage.MainError>(new Dictionary<string, object?>()
                        {
                            { nameof(MainError.ErrorMessage), errorMessage },
                        })
                    {
                        StatusCode = 500
                    };
                }
                else
                {
                    result = new RazorComponentResult<TheStoryVault.Pages.ErrorPage.MainView>(new Dictionary<string, object?>()
                        {
                            { nameof(MainError.ErrorMessage), errorMessage },
                        })
                    {
                        StatusCode = 500
                    };
                }

                await result.ExecuteAsync(context);
            });
        });
    }
}
