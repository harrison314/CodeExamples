using Microsoft.AspNetCore.Http.HttpResults;

namespace TheStoryVault.Pages.EndOfDemo;

internal static class Endpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/endOfDemo", () =>
        {
            return new RazorComponentResult<TheStoryVault.Pages.Order.MainView>(new Dictionary<string, object?>()
            {
            });
        });
    }
}
