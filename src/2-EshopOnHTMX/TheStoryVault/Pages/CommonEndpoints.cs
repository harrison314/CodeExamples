using Microsoft.AspNetCore.Http.HttpResults;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Pages;

internal static class CommonEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/Common/Cookies/{type}", (string type, ICookieApi cookieApi) =>
        {
            if (type == "Necessary")
            {
               cookieApi.SetTrackingIdentifier(false);
            }
            else if (type == "All")
            {
                cookieApi.SetTrackingIdentifier(true);
            }
            else
            {
                //app.Logger.LogError("Bad type {Type} in /Common/Cookies/", type);
                return Results.BadRequest();
            }

            return Results.Content(string.Empty, "text/html");
        });

        app.MapGet("/Common/Cookies", () =>
        {
            return new RazorComponentResult<TheStoryVault.Pages.MainLayoutShared.CookiePanel>();
        });
    }
}
